!function(){"use strict";var t="".replace(/([^/])$/,"$1/"),e=location.pathname,n=e.startsWith(t)&&decodeURI("/".concat(e.slice(t.length)));if(n){var a=document,c=a.head,r=a.createElement.bind(a),i=function(t,e,n){var a,c=e.r[t]||(null===(a=Object.entries(e.r).find((function(e){var n=e[0];return new RegExp("^".concat(n.replace(/\/:[^/]+/g,"/[^/]+").replace("/*","/.+"),"$")).test(t)})))||void 0===a?void 0:a[1]);return null==c?void 0:c.map((function(t){var a=e.f[t][1],c=e.f[t][0];return{type:c.split(".").pop(),url:"".concat(n.publicPath).concat(c),attrs:[["data-".concat(e.b),"".concat(e.p,":").concat(a)]]}}))}(n,{"p":"midjourney-proxy-admin","b":"webpack","f":[["13.a9e4b95d.async.js",13],["44.5874b145.async.js",44],["88.6c5b46ee.async.js",88],["89.600f4609.async.js",89],["102.6b18df08.async.js",102],["120.9b739ae2.async.js",120],["123.a5d237dc.async.js",123],["134.746c1fbc.async.js",134],["p__Welcome.7bf5e365.async.js",185],["p__AccountList__index.b1f7260a.async.js",201],["228.72c78f5a.async.js",228],["248.4228c97c.async.js",248],["t__plugin-layout__Layout.5012e1ab.chunk.css",301],["t__plugin-layout__Layout.d587937a.async.js",301],["311.027fc360.async.js",311],["p__User__Login__index.899b2777.async.js",366],["390.976f7083.async.js",390],["401.61dd42f7.async.js",401],["p__Task__List__index.4ce6e3c9.async.js",502],["p__DomainList__List__index.ef8686b4.async.js",523],["p__404.8182e601.async.js",571],["p__UserList__List__index.936257e7.async.js",579],["695.1d52d6cf.async.js",695],["p__Probe__index.6a9ff373.chunk.css",780],["p__Probe__index.35e86f9d.async.js",780],["818.681127f4.async.js",818],["p__Draw__index.63774c9e.chunk.css",903],["p__Draw__index.1f34316b.async.js",903],["905.75545e72.async.js",905],["958.95169f45.async.js",958],["p__Setting__index.f0efc6e7.async.js",971]],"r":{"/*":[20,28],"/":[3,7,12,13,28],"/welcome":[7,8,10,3,12,13,28],"/account":[0,4,7,9,10,11,14,17,25,29,3,12,13,28],"/domain":[0,1,4,6,7,10,11,17,19,22,25,28,29,3,12,13],"/user-list":[0,1,4,6,7,10,11,17,21,22,25,28,29,3,12,13],"/task":[0,1,4,6,7,10,11,17,18,22,25,28,29,3,12,13],"/draw-test":[2,4,6,7,10,11,17,26,27,3,12,13,28],"/setting":[0,4,5,7,10,30,3,12,13,28],"/probe":[7,10,23,24,3,12,13,28],"/user/login":[0,1,4,6,7,15,17,25]}},{publicPath:"/"});null==i||i.forEach((function(t){var e,n=t.type,a=t.url;if("js"===n)(e=r("script")).src=a,e.async=!0;else{if("css"!==n)return;(e=r("link")).href=a,e.rel="preload",e.as="style"}t.attrs.forEach((function(t){e.setAttribute(t[0],t[1]||"")})),c.appendChild(e)}))}}();