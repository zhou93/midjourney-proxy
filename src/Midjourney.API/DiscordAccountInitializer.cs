﻿using Microsoft.Extensions.Options;
using Midjourney.Infrastructure.Data;
using Midjourney.Infrastructure.LoadBalancer;
using Midjourney.Infrastructure.Services;
using Midjourney.Infrastructure.Util;
using MongoDB.Driver;
using Serilog;

using ILogger = Serilog.ILogger;

namespace Midjourney.API
{
    /// <summary>
    /// Discord账号初始化器，用于初始化Discord账号实例。
    /// </summary>
    public class DiscordAccountInitializer : IHostedService
    {
        private readonly ITaskService _taskService;
        private readonly DiscordLoadBalancer _discordLoadBalancer;
        private readonly DiscordAccountHelper _discordAccountHelper;
        private readonly ProxyProperties _properties;

        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
        private Timer _timer;

        public DiscordAccountInitializer(
            DiscordLoadBalancer discordLoadBalancer,
            DiscordAccountHelper discordAccountHelper,
            IConfiguration configuration,
            IOptions<ProxyProperties> options,
            ITaskService taskService)
        {
            _discordLoadBalancer = discordLoadBalancer;
            _discordAccountHelper = discordAccountHelper;
            _properties = options.Value;
            _taskService = taskService;
            _configuration = configuration;
            _logger = Log.Logger;
        }

        /// <summary>
        /// 启动服务并初始化Discord账号实例。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // 初始化环境变量
            var proxy = GlobalConfiguration.Setting?.Proxy;
            if (!string.IsNullOrEmpty(proxy.Host))
            {
                Environment.SetEnvironmentVariable("http_proxyHost", proxy.Host);
                Environment.SetEnvironmentVariable("http_proxyPort", proxy.Port.ToString());
                Environment.SetEnvironmentVariable("https_proxyHost", proxy.Host);
                Environment.SetEnvironmentVariable("https_proxyPort", proxy.Port.ToString());
            }

            // 初始化管理员用户
            // 判断超管是否存在
            var admin = DbHelper.UserStore.Get(Constants.ADMIN_USER_ID);
            if (admin == null)
            {
                admin = new User
                {
                    Id = Constants.ADMIN_USER_ID,
                    Name = Constants.ADMIN_USER_ID,
                    Token = _configuration["AdminToken"],
                    Role = EUserRole.ADMIN,
                    Status = EUserStatus.NORMAL,
                    IsWhite = true
                };

                if (string.IsNullOrWhiteSpace(admin.Token))
                {
                    admin.Token = "admin";
                }

                DbHelper.UserStore.Add(admin);
            }

            // 初始化普通用户
            var user = DbHelper.UserStore.Get(Constants.DEFAULT_USER_ID);
            var userToken = _configuration["UserToken"];
            if (user == null && !string.IsNullOrWhiteSpace(userToken))
            {
                user = new User
                {
                    Id = Constants.DEFAULT_USER_ID,
                    Name = Constants.DEFAULT_USER_ID,
                    Token = userToken,
                    Role = EUserRole.USER,
                    Status = EUserStatus.NORMAL,
                    IsWhite = true
                };
                DbHelper.UserStore.Add(user);
            }

            // 初始化领域标签
            var defaultDomain = DbHelper.DomainStore.Get(Constants.DEFAULT_DOMAIN_ID);
            if (defaultDomain == null)
            {
                defaultDomain = new DomainTag
                {
                    Id = Constants.DEFAULT_DOMAIN_ID,
                    Name = "默认标签",
                    Description = "",
                    Sort = 0,
                    Enable = true,
                    Keywords = WordsUtils.GetWords()
                };
                DbHelper.DomainStore.Add(defaultDomain);
            }

            // 完整标签
            var fullDomain = DbHelper.DomainStore.Get(Constants.DEFAULT_DOMAIN_FULL_ID);
            if (fullDomain == null)
            {
                fullDomain = new DomainTag
                {
                    Id = Constants.DEFAULT_DOMAIN_FULL_ID,
                    Name = "默认完整标签",
                    Description = "",
                    Sort = 0,
                    Enable = true,
                    Keywords = WordsUtils.GetWordsFull()
                };
                DbHelper.DomainStore.Add(fullDomain);
            }

            // 违规词
            var bannedWord = DbHelper.BannedWordStore.Get(Constants.DEFAULT_BANNED_WORD_ID);
            if (bannedWord == null)
            {
                bannedWord = new BannedWord
                {
                    Id = Constants.DEFAULT_BANNED_WORD_ID,
                    Name = "默认违规词",
                    Description = "",
                    Sort = 0,
                    Enable = true,
                    Keywords = BannedPromptUtils.GetStrings()
                };
                DbHelper.BannedWordStore.Add(bannedWord);
            }

            if (GlobalConfiguration.Setting.IsMongo)
            {
                _ = Task.Run(() =>
                {
                    // 索引
                    MongoIndexInit();

                    // 自动迁移 task 数据
                    if (GlobalConfiguration.Setting.IsMongoAutoMigrate)
                    {
                        MongoAutoMigrate();
                    }
                });
            }

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

            await Task.CompletedTask;
        }

        /// <summary>
        /// 初始化 mongodb 索引和最大数据限制
        /// </summary>
        public void MongoIndexInit()
        {
            try
            {
                if (!GlobalConfiguration.Setting.IsMongo)
                {
                    return;
                }

                LocalLock.TryLock("MongoIndexInit", TimeSpan.FromSeconds(10), () =>
                {
                    // 不能固定大小，因为无法修改数据
                    //var database = MongoHelper.Instance;
                    //var collectionName = "task";
                    //var collectionExists = database.ListCollectionNames().ToList().Contains(collectionName);
                    //if (!collectionExists)
                    //{
                    //    var options = new CreateCollectionOptions
                    //    {
                    //        Capped = true,
                    //        MaxSize = 1024L * 1024L * 1024L * 1024L,  // 1 TB 的集合大小，实际上不受大小限制
                    //        MaxDocuments = 1000000
                    //    };
                    //    database.CreateCollection("task", options);
                    //}

                    var coll = MongoHelper.GetCollection<TaskInfo>();

                    var index1 = new CreateIndexModel<TaskInfo>(Builders<TaskInfo>.IndexKeys.Descending(c => c.SubmitTime));
                    coll.Indexes.CreateOne(index1);

                    var index2 = new CreateIndexModel<TaskInfo>(Builders<TaskInfo>.IndexKeys.Ascending(c => c.PromptEn));
                    coll.Indexes.CreateOne(index2);

                    var index3 = new CreateIndexModel<TaskInfo>(Builders<TaskInfo>.IndexKeys.Descending(c => c.Prompt));
                    coll.Indexes.CreateOne(index3);

                    var index4 = new CreateIndexModel<TaskInfo>(Builders<TaskInfo>.IndexKeys.Ascending(c => c.InstanceId));
                    coll.Indexes.CreateOne(index4);

                    var index5 = new CreateIndexModel<TaskInfo>(Builders<TaskInfo>.IndexKeys.Ascending(c => c.Status));
                    coll.Indexes.CreateOne(index5);

                    var index6 = new CreateIndexModel<TaskInfo>(Builders<TaskInfo>.IndexKeys.Ascending(c => c.Action));
                    coll.Indexes.CreateOne(index6);

                    var index7 = new CreateIndexModel<TaskInfo>(Builders<TaskInfo>.IndexKeys.Ascending(c => c.Description));
                    coll.Indexes.CreateOne(index7);

                    var index8 = new CreateIndexModel<TaskInfo>(Builders<TaskInfo>.IndexKeys.Ascending(c => c.Description));
                    coll.Indexes.CreateOne(index8);
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "初始化 mongodb 索引和最大数据限制异常");
            }
        }

        /// <summary>
        /// 自动迁移 task 数据
        /// </summary>
        public void MongoAutoMigrate()
        {
            try
            {
                LocalLock.TryLock("MongoAutoMigrate", TimeSpan.FromSeconds(10), () =>
                {
                    // 判断最后一条是否存在
                    var success = 0;
                    var last = DbHelper.TaskStore.GetCollection().Query().OrderByDescending(c => c.SubmitTime).FirstOrDefault();
                    if (last != null)
                    {
                        var coll = MongoHelper.GetCollection<TaskInfo>();
                        var lastMongo = coll.Find(c => c.Id == last.Id).FirstOrDefault();
                        if (lastMongo == null)
                        {
                            // 迁移数据
                            var taskIds = DbHelper.TaskStore.GetCollection().Query().Select(c => c.Id).ToList();
                            foreach (var tid in taskIds)
                            {
                                var info = DbHelper.TaskStore.Get(tid);
                                if (info != null)
                                {
                                    // 判断是否存在
                                    var exist = coll.CountDocuments(c => c.Id == info.Id) > 0;
                                    if (!exist)
                                    {
                                        coll.InsertOne(info);
                                        success++;
                                    }
                                }
                            }

                            _logger.Information("MongoAutoMigrate success: {@0}", success);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "MongoAutoMigrate error");
            }
        }

        private async void DoWork(object state)
        {
            if (_semaphoreSlim.CurrentCount == 0)
            {
                return;
            }

            await _semaphoreSlim.WaitAsync();

            try
            {
                _logger.Information("开始例行检查");

                // 配置中的默认账号，如果存在
                var configAccounts = _properties.Accounts.ToList();
                if (!string.IsNullOrEmpty(_properties.Discord?.ChannelId) && _properties.Discord.ChannelId.EndsWith("***"))
                {
                    configAccounts.Add(_properties.Discord);
                }

                await Initialize(configAccounts.ToArray());

                CheckAndDeleteOldDocuments();

                _logger.Information("例行检查完成");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "执行例行检查时发生异常");
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        /// <summary>
        /// 检查并删除旧文档
        /// </summary>
        public static void CheckAndDeleteOldDocuments()
        {
            if (GlobalConfiguration.Setting.MaxCount <= 0)
            {
                return;
            }

            var maxCount = GlobalConfiguration.Setting.MaxCount;

            // 如果超过 x 条，删除最早插入的数据
            if (GlobalConfiguration.Setting.IsMongo)
            {
                var coll = MongoHelper.GetCollection<TaskInfo>();
                var documentCount = coll.CountDocuments(Builders<TaskInfo>.Filter.Empty);
                if (documentCount > maxCount)
                {
                    var documentsToDelete = documentCount - maxCount;
                    var ids = coll.Find(c => true).SortBy(c => c.SubmitTime).Limit((int)documentsToDelete).Project(c => c.Id).ToList();
                    if (ids.Any())
                    {
                        coll.DeleteMany(c => ids.Contains(c.Id));
                    }
                }
            }
            else
            {
                var documentCount = DbHelper.TaskStore.GetCollection().Query().Count();
                if (documentCount > maxCount)
                {
                    var documentsToDelete = documentCount - maxCount;
                    var ids = DbHelper.TaskStore.GetCollection().Query().OrderBy(c => c.SubmitTime)
                        .Limit(documentsToDelete)
                        .ToList()
                        .Select(c => c.Id);

                    if (ids.Any())
                    {
                        DbHelper.TaskStore.GetCollection().DeleteMany(c => ids.Contains(c.Id));
                    }
                }
            }
        }

        /// <summary>
        /// 初始化所有账号
        /// </summary>
        /// <returns></returns>
        public async Task Initialize(params DiscordAccountConfig[] appends)
        {
            var isLock = await AsyncLocalLock.TryLockAsync("Initialize", TimeSpan.FromSeconds(10), async () =>
            {
                var db = DbHelper.AccountStore;
                var accounts = db.GetAll().OrderBy(c => c.Sort).ToList();

                // 将启动配置中的 account 添加到数据库
                var configAccounts = new List<DiscordAccountConfig>();
                if (appends?.Length > 0)
                {
                    configAccounts.AddRange(appends);
                }

                foreach (var configAccount in configAccounts)
                {
                    var account = accounts.FirstOrDefault(c => c.ChannelId == configAccount.ChannelId);
                    if (account == null)
                    {
                        if (configAccount.Interval < 1.2m)
                        {
                            configAccount.Interval = 1.2m;
                        }

                        account = new DiscordAccount
                        {
                            Id = configAccount.ChannelId,
                            ChannelId = configAccount.ChannelId,

                            GuildId = configAccount.GuildId,
                            UserToken = configAccount.UserToken,
                            UserAgent = string.IsNullOrEmpty(configAccount.UserAgent) ? Constants.DEFAULT_DISCORD_USER_AGENT : configAccount.UserAgent,
                            Enable = configAccount.Enable,
                            CoreSize = configAccount.CoreSize,
                            QueueSize = configAccount.QueueSize,
                            BotToken = configAccount.BotToken,
                            TimeoutMinutes = configAccount.TimeoutMinutes,
                            PrivateChannelId = configAccount.PrivateChannelId,
                            NijiBotChannelId = configAccount.NijiBotChannelId,
                            MaxQueueSize = configAccount.MaxQueueSize,
                            Mode = configAccount.Mode,
                            AllowModes = configAccount.AllowModes,
                            Weight = configAccount.Weight,
                            Remark = configAccount.Remark,
                            RemixAutoSubmit = configAccount.RemixAutoSubmit,
                            Sponsor = configAccount.Sponsor,
                            Sort = configAccount.Sort,
                            Interval = configAccount.Interval,

                            AfterIntervalMax = configAccount.AfterIntervalMax,
                            AfterIntervalMin = configAccount.AfterIntervalMin,
                            WorkTime = configAccount.WorkTime,
                            FishingTime = configAccount.FishingTime,
                            PermanentInvitationLink = configAccount.PermanentInvitationLink,

                            SubChannels = configAccount.SubChannels,
                            IsBlend = configAccount.IsBlend,
                            VerticalDomainIds = configAccount.VerticalDomainIds,
                            IsVerticalDomain = configAccount.IsVerticalDomain,
                            IsDescribe = configAccount.IsDescribe,
                            DayDrawLimit = configAccount.DayDrawLimit,
                            EnableMj = configAccount.EnableMj,
                            EnableNiji = configAccount.EnableNiji,
                            EnableFastToRelax = configAccount.EnableFastToRelax
                        };

                        db.Add(account);
                        accounts.Add(account);
                    }
                }

                var instances = _discordLoadBalancer.GetAllInstances();
                foreach (var account in accounts)
                {
                    if (account.Enable != true)
                    {
                        continue;
                    }

                    IDiscordInstance disInstance = null;
                    try
                    {
                        disInstance = _discordLoadBalancer.GetDiscordInstance(account.ChannelId);

                        // 判断是否在工作时间内
                        var now = new DateTimeOffset(DateTime.Now.Date).ToUnixTimeMilliseconds();
                        var dayCount = (int)TaskHelper.Instance.TaskStore.Count(c => c.InstanceId == account.ChannelId && c.SubmitTime >= now);

                        if (DateTime.Now.IsInWorkTime(account.WorkTime)
                        && (account.DayDrawLimit < 0 || dayCount < account.DayDrawLimit))
                        {
                            if (disInstance == null)
                            {
                                // 启动前校验
                                if (account.SubChannels.Count > 0)
                                {
                                    // https://discord.com/channels/1256526716130693201/1256526716130693204
                                    // https://discord.com/channels/{guid}/{id}
                                    // {guid} {id} 都是纯数字

                                    var dic = new Dictionary<string, string>();
                                    foreach (var item in account.SubChannels)
                                    {
                                        if (string.IsNullOrWhiteSpace(item) || !item.Contains("https://discord.com/channels"))
                                        {
                                            continue;
                                        }

                                        // {id} 作为 key, {guid} 作为 value
                                        var fir = item.Split(',').Where(c => c.Contains("https://discord.com/channels")).FirstOrDefault();
                                        if (fir == null)
                                        {
                                            continue;
                                        }

                                        var arr = fir.Split('/').Where(c => !string.IsNullOrWhiteSpace(c)).ToArray();
                                        if (arr.Length < 5)
                                        {
                                            continue;
                                        }

                                        var guid = arr[3];
                                        var id = arr[4];

                                        dic[id] = guid;
                                    }

                                    account.SubChannelValues = dic;
                                }
                                else
                                {
                                    account.SubChannels.Clear();
                                    account.SubChannelValues.Clear();
                                }

                                account.DayDrawCount = dayCount;
                                db.Update(account);

                                disInstance = await _discordAccountHelper.CreateDiscordInstance(account);
                                instances.Add(disInstance);
                                _discordLoadBalancer.AddInstance(disInstance);

                                // 这里应该等待初始化完成，并获取用户信息验证，获取用户成功后设置为可用状态
                                // 多账号启动时，等待一段时间再启动下一个账号
                                await Task.Delay(1000 * 5);

                                // 启动后执行 info setting 操作
                                await _taskService.InfoSetting(account.ChannelId);
                            }
                        }
                        else
                        {
                            // 非工作时间内，如果存在实例则释放
                            if (disInstance != null)
                            {
                                _discordLoadBalancer.RemoveInstance(disInstance);

                                disInstance.Dispose();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Account({@0}) init fail, disabled: {@1}", account.GetDisplay(), ex.Message);

                        account.Enable = false;
                        account.DisabledReason = "初始化失败";

                        db.Update(account);

                        disInstance?.ClearAccountCache(account.Id);
                    }
                }

                var enableInstanceIds = instances.Where(instance => instance.IsAlive)
                    .Select(instance => instance.GetInstanceId)
                    .ToHashSet();

                _logger.Information("当前可用账号数 [{@0}] - {@1}", enableInstanceIds.Count, string.Join(", ", enableInstanceIds));
            });
            if (!isLock)
            {
                throw new LogicException("初始化中，请稍后重拾");
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// 更新账号信息
        /// </summary>
        /// <param name="param"></param>
        public void UpdateAccount(DiscordAccount param)
        {
            DiscordAccount model = null;

            var disInstance = _discordLoadBalancer.GetDiscordInstance(param.ChannelId);
            if (disInstance != null)
            {
                model = disInstance.Account;
            }

            if (model == null)
            {
                model = DbHelper.AccountStore.Get(param.Id);
            }

            if (model == null)
            {
                throw new LogicException("账号不存在");
            }

            // 渠道 ID 和 服务器 ID 禁止修改
            //model.ChannelId = account.ChannelId;
            //model.GuildId = account.GuildId;

            // 更新账号重连时，自动解锁
            model.Lock = false;
            model.CfHashCreated = null;
            model.CfHashUrl = null;
            model.CfUrl = null;

            // 验证 Interval
            if (param.Interval < 1.2m)
            {
                param.Interval = 1.2m;
            }

            // 验证 WorkTime
            if (!string.IsNullOrEmpty(param.WorkTime))
            {
                var ts = param.WorkTime.ToTimeSlots();
                if (ts.Count == 0)
                {
                    param.WorkTime = null;
                }
            }

            // 验证 FishingTime
            if (!string.IsNullOrEmpty(param.FishingTime))
            {
                var ts = param.FishingTime.ToTimeSlots();
                if (ts.Count == 0)
                {
                    param.FishingTime = null;
                }
            }

            model.EnableFastToRelax = param.EnableFastToRelax;
            model.IsBlend = param.IsBlend;
            model.IsDescribe = param.IsDescribe;
            model.DayDrawLimit = param.DayDrawLimit;
            model.IsVerticalDomain = param.IsVerticalDomain;
            model.VerticalDomainIds = param.VerticalDomainIds;
            model.SubChannels = param.SubChannels;

            model.PermanentInvitationLink = param.PermanentInvitationLink;
            model.FishingTime = param.FishingTime;
            model.EnableNiji = param.EnableNiji;
            model.EnableMj = param.EnableMj;
            model.AllowModes = param.AllowModes;
            model.WorkTime = param.WorkTime;
            model.Interval = param.Interval;
            model.AfterIntervalMin = param.AfterIntervalMin;
            model.AfterIntervalMax = param.AfterIntervalMax;
            model.Sort = param.Sort;
            model.Enable = param.Enable;
            model.PrivateChannelId = param.PrivateChannelId;
            model.NijiBotChannelId = param.NijiBotChannelId;
            model.UserAgent = param.UserAgent;
            model.RemixAutoSubmit = param.RemixAutoSubmit;
            model.CoreSize = param.CoreSize;
            model.QueueSize = param.QueueSize;
            model.MaxQueueSize = param.MaxQueueSize;
            model.TimeoutMinutes = param.TimeoutMinutes;
            model.Weight = param.Weight;
            model.Remark = param.Remark;
            model.BotToken = param.BotToken;
            model.UserToken = param.UserToken;
            model.Mode = param.Mode;
            model.Sponsor = param.Sponsor;

            DbHelper.AccountStore.Update(model);

            disInstance?.ClearAccountCache(model.Id);
        }

        /// <summary>
        /// 更新并重新连接账号
        /// </summary>
        /// <param name="account"></param>
        public async Task ReconnectAccount(DiscordAccount account)
        {
            try
            {
                // 如果正在执行则释放
                var disInstance = _discordLoadBalancer.GetDiscordInstance(account.ChannelId);
                if (disInstance != null)
                {
                    _discordLoadBalancer.RemoveInstance(disInstance);

                    disInstance.Dispose();
                }
            }
            catch
            {
            }

            UpdateAccount(account);

            await Initialize();
        }

        /// <summary>
        /// 停止连接并删除账号
        /// </summary>
        /// <param name="id"></param>
        public void DeleteAccount(string id)
        {
            try
            {
                var disInstance = _discordLoadBalancer.GetDiscordInstance(id);
                if (disInstance != null)
                {
                    disInstance.Dispose();
                }
            }
            catch
            {
            }

            var model = DbHelper.AccountStore.Get(id);
            if (model != null)
            {
                DbHelper.AccountStore.Delete(id);
            }
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Information("例行检查服务已停止");

            _timer?.Change(Timeout.Infinite, 0);
            await _semaphoreSlim.WaitAsync();
            _semaphoreSlim.Release();
        }
    }
}