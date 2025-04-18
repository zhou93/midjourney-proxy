"use strict";(self.webpackChunkmidjourney_proxy_admin=self.webpackChunkmidjourney_proxy_admin||[]).push([[523],{47389:function(R,f,e){var M=e(1413),m=e(67294),i=e(27363),u=e(91146),d=function(r,D){return m.createElement(u.Z,(0,M.Z)((0,M.Z)({},r),{},{ref:D,icon:i.Z}))},c=m.forwardRef(d);f.Z=c},51042:function(R,f,e){var M=e(1413),m=e(67294),i=e(42110),u=e(91146),d=function(r,D){return m.createElement(u.Z,(0,M.Z)((0,M.Z)({},r),{},{ref:D,icon:i.Z}))},c=m.forwardRef(d);f.Z=c},9829:function(R,f,e){e.r(f);var M=e(97857),m=e.n(M),i=e(15009),u=e.n(i),d=e(99289),c=e.n(d),P=e(5574),r=e.n(P),D=e(27986),h=e(84436),v=e(66927),a=e(47389),L=e(82061),Z=e(51042),I=e(90930),U=e(51967),W=e(94272),C=e(53025),p=e(16568),y=e(83622),T=e(72269),S=e(42075),A=e(86738),ee=e(4393),j=e(67294),n=e(85893),se=function(){var ne=(0,j.useState)(!1),w=r()(ne,2),te=w[0],z=w[1],ae=(0,j.useState)({}),F=r()(ae,2),_e=F[0],Y=F[1],re=(0,j.useState)(""),$=r()(re,2),oe=$[0],ie=$[1],de=(0,j.useState)({}),V=r()(de,2),le=V[0],N=V[1],ue=(0,j.useState)(1e3),G=r()(ue,2),me=G[0],Ee=G[1],Me=C.Z.useForm(),ce=r()(Me,1),x=ce[0],_=(0,W.useIntl)(),fe=p.ZP.useNotification(),H=r()(fe,2),b=H[0],Pe=H[1],B=(0,j.useRef)(),K=function(){Y({}),N({}),z(!1)},Oe=(0,j.useState)(!1),J=r()(Oe,2),De=J[0],Q=J[1],X=(0,n.jsxs)(n.Fragment,{children:[(0,n.jsx)(y.ZP,{onClick:K,children:_.formatMessage({id:"pages.cancel"})},"back"),(0,n.jsx)(y.ZP,{type:"primary",loading:De,onClick:function(){return x.submit()},children:_.formatMessage({id:"pages.submit"})},"submit")]}),k=function(){var E=c()(u()().mark(function l(s){var o,O;return u()().wrap(function(t){for(;;)switch(t.prev=t.next){case 0:return Q(!0),s.keywords||(s.keywords=[]),typeof s.keywords=="string"&&(s.keywords=s.keywords.split(",")),t.next=5,(0,v.Fj)(s);case 5:o=t.sent,o.success?(b.success({message:"success",description:o.message}),K(),(O=B.current)===null||O===void 0||O.reload()):b.error({message:"error",description:o.message}),Q(!1);case 8:case"end":return t.stop()}},l)}));return function(s){return E.apply(this,arguments)}}(),q=function(l,s,o,O){x.resetFields(),ie(l),Y(s),N(o),Ee(O),z(!0)},pe=function(){var E=c()(u()().mark(function l(s){var o,O;return u()().wrap(function(t){for(;;)switch(t.prev=t.next){case 0:return t.prev=0,t.next=3,(0,v.dj)(s);case 3:o=t.sent,o.success?(b.success({message:"success",description:_.formatMessage({id:"pages.task.deleteSuccess"})}),(O=B.current)===null||O===void 0||O.reload()):b.error({message:"error",description:o.message}),t.next=10;break;case 7:t.prev=7,t.t0=t.catch(0),console.error(t.t0);case 10:return t.prev=10,t.finish(10);case 12:case"end":return t.stop()}},l,null,[[0,7,10,12]])}));return function(s){return E.apply(this,arguments)}}(),ge=[{title:_.formatMessage({id:"pages.domain.name"}),dataIndex:"name",width:160,align:"left",fixed:"left"},{title:_.formatMessage({id:"pages.domain.description"}),dataIndex:"description",width:160,align:"left"},{title:_.formatMessage({id:"pages.domain.keywords"}),dataIndex:"keywords",width:120,align:"left",render:function(l,s){return(0,n.jsxs)("span",{children:[(s==null?void 0:s.keywords.length)||"0"," ",_.formatMessage({id:"pages.domain.keywords"})]})}},{title:_.formatMessage({id:"pages.domain.enable"}),dataIndex:"enable",width:80,showInfo:!1,hideInSearch:!0,align:"left",render:function(l,s){return(0,n.jsx)(T.Z,{disabled:!0,checked:s.enable})}},{title:_.formatMessage({id:"pages.domain.weight"}),dataIndex:"weight",width:80,align:"left",hideInSearch:!0},{title:_.formatMessage({id:"pages.domain.createTime"}),dataIndex:"createTimeFormat",width:140,ellipsis:!0,hideInSearch:!0},{title:_.formatMessage({id:"pages.domain.updateTime"}),dataIndex:"updateTimeFormat",width:140,ellipsis:!0,hideInSearch:!0},{title:_.formatMessage({id:"pages.operation"}),dataIndex:"operation",width:140,key:"operation",fixed:"right",align:"center",hideInSearch:!0,render:function(l,s){return(0,n.jsxs)(S.Z,{children:[(0,n.jsx)(y.ZP,{icon:(0,n.jsx)(a.Z,{}),onClick:function(){return q(_.formatMessage({id:"pages.domain.update"}),(0,n.jsx)(h.Z,{form:x,record:s,onSubmit:k}),X,1e3)}},"Update"),(0,n.jsx)(A.Z,{title:_.formatMessage({id:"pages.domain.delete"}),description:_.formatMessage({id:"pages.domain.deleteTitle"}),onConfirm:function(){return pe(s.id)},children:(0,n.jsx)(y.ZP,{danger:!0,icon:(0,n.jsx)(L.Z,{})})})]})}}];return(0,n.jsxs)(I._z,{children:[Pe,(0,n.jsx)(ee.Z,{children:(0,n.jsx)(U.Z,{columns:ge,scroll:{x:1e3},search:{defaultCollapsed:!0},pagination:{pageSize:10,showQuickJumper:!1,showSizeChanger:!1},rowKey:"id",actionRef:B,toolBarRender:function(){return[(0,n.jsx)(y.ZP,{type:"primary",icon:(0,n.jsx)(Z.Z,{}),onClick:function(){return q(_.formatMessage({id:"pages.domain.add"}),(0,n.jsx)(h.Z,{form:x,record:{},onSubmit:k}),X,1e3)},children:_.formatMessage({id:"pages.add"})},"primary")]},request:function(){var E=c()(u()().mark(function l(s){var o;return u()().wrap(function(g){for(;;)switch(g.prev=g.next){case 0:return s.keywords&&typeof s.keywords=="string"&&(s.keywords=s.keywords.split(",")),g.next=3,(0,v.Kh)(m()(m()({},s),{},{pageNumber:s.current-1}));case 3:return o=g.sent,g.abrupt("return",{data:o.list,total:o.pagination.total,success:!0});case 5:case"end":return g.stop()}},l)}));return function(l){return E.apply(this,arguments)}}()})}),(0,n.jsx)(D.Z,{title:oe,modalVisible:te,hideModal:K,modalContent:_e,footer:le,modalWidth:me})]})};f.default=se},84436:function(R,f,e){var M=e(5574),m=e.n(M),i=e(53025),u=e(71230),d=e(15746),c=e(4393),P=e(55102),r=e(72269),D=e(37804),h=e(67294),v=e(94272),a=e(85893),L=function(I){var U=I.form,W=I.onSubmit,C=I.record,p=(0,v.useIntl)(),y=(0,h.useState)(!1),T=m()(y,2),S=T[0],A=T[1];return(0,h.useEffect)(function(){U.setFieldsValue(C),C.id=="admin"?A(!0):A(!1),console.log("record",C)}),(0,a.jsx)(i.Z,{form:U,labelAlign:"left",layout:"horizontal",labelCol:{span:4},wrapperCol:{span:20},onFinish:W,children:(0,a.jsx)(u.Z,{gutter:16,children:(0,a.jsx)(d.Z,{span:24,children:(0,a.jsxs)(c.Z,{type:"inner",children:[(0,a.jsx)(i.Z.Item,{label:"id",name:"id",hidden:!0,children:(0,a.jsx)(P.Z,{})}),(0,a.jsx)(i.Z.Item,{required:!0,label:p.formatMessage({id:"pages.domain.name"}),name:"name",children:(0,a.jsx)(P.Z,{})}),(0,a.jsx)(i.Z.Item,{label:p.formatMessage({id:"pages.domain.description"}),name:"description",children:(0,a.jsx)(P.Z,{})}),(0,a.jsx)(i.Z.Item,{label:p.formatMessage({id:"pages.domain.enable"}),name:"enable",children:(0,a.jsx)(r.Z,{})}),(0,a.jsx)(i.Z.Item,{label:p.formatMessage({id:"pages.domain.sort"}),name:"sort",children:(0,a.jsx)(D.Z,{})}),(0,a.jsx)(i.Z.Item,{label:p.formatMessage({id:"pages.domain.keywords"}),name:"keywords",tooltip:p.formatMessage({id:"pages.domain.keywords.tooltip"}),children:(0,a.jsx)(P.Z.TextArea,{placeholder:"cat, dog, fish",autoSize:{minRows:1,maxRows:20},allowClear:!0})})]})})})})};f.Z=L},27986:function(R,f,e){var M=e(28248),m=e(85893),i=function(d){var c=d.title,P=d.modalVisible,r=d.hideModal,D=d.modalContent,h=d.footer,v=d.modalWidth;return(0,m.jsx)(M.Z,{title:c,open:P,onCancel:r,footer:h,width:v,children:D})};f.Z=i}}]);
