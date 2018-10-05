/* Copyright 2017 Mozilla Foundation
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
 // pdfjs 2018-01-18 2
if(!("undefined"!==typeof PDFJSDev&&PDFJSDev.test("FIREFOX || MOZCENTRAL || CHROME")||"undefined"!==typeof PDFJS&&PDFJS.compatibilityChecked)){var globalScope=require("./global_scope"),isNodeJS=require("./is_node"),userAgent="undefined"!==typeof navigator&&navigator.userAgent||"",isAndroid=/Android/.test(userAgent),isIOSChrome=0<=userAgent.indexOf("CriOS"),isIE=0<=userAgent.indexOf("Trident"),isIOS=/\b(iPad|iPhone|iPod)(?=;)/.test(userAgent),isSafari=/Safari\//.test(userAgent)&&!/(Chrome\/|Android\s)/.test(userAgent),
hasDOM="object"===typeof window&&"object"===typeof document;"undefined"===typeof PDFJS&&(globalScope.PDFJS={});PDFJS.compatibilityChecked=!0;(function(){!globalScope.btoa&&isNodeJS()&&(globalScope.btoa=function(m){return Buffer.from(m,"binary").toString("base64")})})();(function(){!globalScope.atob&&isNodeJS()&&(globalScope.atob=function(m){return Buffer.from(m,"base64").toString("binary")})})();(function(){if(isIE||isIOSChrome)PDFJS.disableCreateObjectURL=!0})();(function(){"undefined"===typeof navigator||
"language"in navigator||(PDFJS.locale=navigator.userLanguage||"en-US")})();(function(){if(isSafari||isIOS)PDFJS.disableRange=!0,PDFJS.disableStream=!0})();(function(){if(isIOS||isAndroid)PDFJS.maxCanvasPixels=5242880})();(function(){hasDOM&&isIE&&window.parent!==window&&(PDFJS.disableFullscreen=!0)})();(function(){hasDOM&&("currentScript"in document||Object.defineProperty(document,"currentScript",{get:function(){var m=document.getElementsByTagName("script");return m[m.length-1]},enumerable:!0,configurable:!0}))})();
(function(){hasDOM&&"undefined"===typeof Element.prototype.remove&&(Element.prototype.remove=function(){this.parentNode&&this.parentNode.removeChild(this)})})();(function(){Object.values||(Object.values=require("core-js/fn/object/values"))})();(function(){Array.prototype.includes||(Array.prototype.includes=require("core-js/fn/array/includes"))})();(function(){Math.log2||(Math.log2=require("core-js/fn/math/log2"))})();(function(){Number.isNaN||(Number.isNaN=require("core-js/fn/number/is-nan"))})();
(function(){Number.isInteger||(Number.isInteger=require("core-js/fn/number/is-integer"))})();(function(){globalScope.Promise||(globalScope.Promise=require("core-js/fn/promise"))})();(function(){globalScope.WeakMap||(globalScope.WeakMap=require("core-js/fn/weak-map"))})();(function(){function m(b){""===b&&(r.call(this),this._isInvalid=!0);return b.toLowerCase()}function w(b){var f=b.charCodeAt(0);return 32<f&&127>f&&-1===[34,35,60,62,63,96].indexOf(f)?b:encodeURIComponent(b)}function y(b){var f=b.charCodeAt(0);
return 32<f&&127>f&&-1===[34,35,60,62,96].indexOf(f)?b:encodeURIComponent(b)}function g(b,f,e){function h(a){v.push(a)}var d=f||"scheme start",g=0,c="",p=!1,q=!1,v=[];a:for(;(b[g-1]!==l||0===g)&&!this._isInvalid;){var a=b[g];switch(d){case "scheme start":if(a&&u.test(a))c+=a.toLowerCase(),d="scheme";else if(f){h("Invalid scheme.");break a}else{c="";d="no scheme";continue}break;case "scheme":if(a&&z.test(a))c+=a.toLowerCase();else if(":"===a){this._scheme=c;c="";if(f)break a;void 0!==n[this._scheme]&&
(this._isRelative=!0);d="file"===this._scheme?"relative":this._isRelative&&e&&e._scheme===this._scheme?"relative or authority":this._isRelative?"authority first slash":"scheme data"}else if(f){a!==l&&h("Code point not allowed in scheme: "+a);break a}else{c="";g=0;d="no scheme";continue}break;case "scheme data":"?"===a?(this._query="?",d="query"):"#"===a?(this._fragment="#",d="fragment"):a!==l&&"\t"!==a&&"\n"!==a&&"\r"!==a&&(this._schemeData+=w(a));break;case "no scheme":if(e&&void 0!==n[e._scheme]){d=
"relative";continue}else h("Missing scheme."),r.call(this),this._isInvalid=!0;break;case "relative or authority":if("/"===a&&"/"===b[g+1])d="authority ignore slashes";else{h("Expected /, got: "+a);d="relative";continue}break;case "relative":this._isRelative=!0;"file"!==this._scheme&&(this._scheme=e._scheme);if(a===l){this._host=e._host;this._port=e._port;this._path=e._path.slice();this._query=e._query;this._username=e._username;this._password=e._password;break a}else if("/"===a||"\\"===a)"\\"===a&&
h("\\ is an invalid code point."),d="relative slash";else if("?"===a)this._host=e._host,this._port=e._port,this._path=e._path.slice(),this._query="?",this._username=e._username,this._password=e._password,d="query";else if("#"===a)this._host=e._host,this._port=e._port,this._path=e._path.slice(),this._query=e._query,this._fragment="#",this._username=e._username,this._password=e._password,d="fragment";else{d=b[g+1];var k=b[g+2];if("file"!==this._scheme||!u.test(a)||":"!==d&&"|"!==d||k!==l&&"/"!==k&&
"\\"!==k&&"?"!==k&&"#"!==k)this._host=e._host,this._port=e._port,this._username=e._username,this._password=e._password,this._path=e._path.slice(),this._path.pop();d="relative path";continue}break;case "relative slash":if("/"===a||"\\"===a)"\\"===a&&h("\\ is an invalid code point."),d="file"===this._scheme?"file host":"authority ignore slashes";else{"file"!==this._scheme&&(this._host=e._host,this._port=e._port,this._username=e._username,this._password=e._password);d="relative path";continue}break;
case "authority first slash":if("/"===a)d="authority second slash";else{h("Expected '/', got: "+a);d="authority ignore slashes";continue}break;case "authority second slash":d="authority ignore slashes";if("/"!==a){h("Expected '/', got: "+a);continue}break;case "authority ignore slashes":if("/"!==a&&"\\"!==a){d="authority";continue}else h("Expected authority, got: "+a);break;case "authority":if("@"===a){p&&(h("@ already seen."),c+="%40");p=!0;for(a=0;a<c.length;a++)k=c[a],"\t"===k||"\n"===k||"\r"===
k?h("Invalid whitespace in authority."):":"===k&&null===this._password?this._password="":(k=w(k),null!==this._password?this._password+=k:this._username+=k);c=""}else if(a===l||"/"===a||"\\"===a||"?"===a||"#"===a){g-=c.length;c="";d="host";continue}else c+=a;break;case "file host":if(a===l||"/"===a||"\\"===a||"?"===a||"#"===a){2!==c.length||!u.test(c[0])||":"!==c[1]&&"|"!==c[1]?(0!==c.length&&(this._host=m.call(this,c),c=""),d="relative path start"):d="relative path";continue}else"\t"===a||"\n"===
a||"\r"===a?h("Invalid whitespace in file host."):c+=a;break;case "host":case "hostname":if(":"!==a||q)if(a===l||"/"===a||"\\"===a||"?"===a||"#"===a){this._host=m.call(this,c);c="";d="relative path start";if(f)break a;continue}else"\t"!==a&&"\n"!==a&&"\r"!==a?("["===a?q=!0:"]"===a&&(q=!1),c+=a):h("Invalid code point in host/hostname: "+a);else if(this._host=m.call(this,c),c="",d="port","hostname"===f)break a;break;case "port":if(/[0-9]/.test(a))c+=a;else if(a===l||"/"===a||"\\"===a||"?"===a||"#"===
a||f){""!==c&&(c=parseInt(c,10),c!==n[this._scheme]&&(this._port=c+""),c="");if(f)break a;d="relative path start";continue}else"\t"===a||"\n"===a||"\r"===a?h("Invalid code point in port: "+a):(r.call(this),this._isInvalid=!0);break;case "relative path start":"\\"===a&&h("'\\' not allowed in path.");d="relative path";if("/"!==a&&"\\"!==a)continue;break;case "relative path":if(a!==l&&"/"!==a&&"\\"!==a&&(f||"?"!==a&&"#"!==a))"\t"!==a&&"\n"!==a&&"\r"!==a&&(c+=w(a));else{"\\"===a&&h("\\ not allowed in relative path.");
if(k=t[c.toLowerCase()])c=k;".."===c?(this._path.pop(),"/"!==a&&"\\"!==a&&this._path.push("")):"."===c&&"/"!==a&&"\\"!==a?this._path.push(""):"."!==c&&("file"===this._scheme&&0===this._path.length&&2===c.length&&u.test(c[0])&&"|"===c[1]&&(c=c[0]+":"),this._path.push(c));c="";"?"===a?(this._query="?",d="query"):"#"===a&&(this._fragment="#",d="fragment")}break;case "query":f||"#"!==a?a!==l&&"\t"!==a&&"\n"!==a&&"\r"!==a&&(this._query+=y(a)):(this._fragment="#",d="fragment");break;case "fragment":a!==
l&&"\t"!==a&&"\n"!==a&&"\r"!==a&&(this._fragment+=a)}g++}}function r(){this._username=this._schemeData=this._scheme="";this._password=null;this._port=this._host="";this._path=[];this._fragment=this._query="";this._isRelative=this._isInvalid=!1}function p(b,f){void 0===f||f instanceof p||(f=new p(String(f)));this._url=b;r.call(this);b=b.replace(/^[ \t\r\n\f]+|[ \t\r\n\f]+$/g,"");g.call(this,b,null,f)}var v=!1;try{if("function"===typeof URL&&"object"===typeof URL.prototype&&"origin"in URL.prototype){var x=
new URL("b","http://a");x.pathname="c%20d";v="http://a/c%20d"===x.href}}catch(b){}if(!v){var n=Object.create(null);n.ftp=21;n.file=0;n.gopher=70;n.http=80;n.https=443;n.ws=80;n.wss=443;var t=Object.create(null);t["%2e"]=".";t[".%2e"]="..";t["%2e."]="..";t["%2e%2e"]="..";var l,u=/[a-zA-Z]/,z=/[a-zA-Z0-9\+\-\.]/;p.prototype={toString:function(){return this.href},get href(){if(this._isInvalid)return this._url;var b="";if(""!==this._username||null!==this._password)b=this._username+(null!==this._password?
":"+this._password:"")+"@";return this.protocol+(this._isRelative?"//"+b+this.host:"")+this.pathname+this._query+this._fragment},set href(b){r.call(this);g.call(this,b)},get protocol(){return this._scheme+":"},set protocol(b){this._isInvalid||g.call(this,b+":","scheme start")},get host(){return this._isInvalid?"":this._port?this._host+":"+this._port:this._host},set host(b){!this._isInvalid&&this._isRelative&&g.call(this,b,"host")},get hostname(){return this._host},set hostname(b){!this._isInvalid&&
this._isRelative&&g.call(this,b,"hostname")},get port(){return this._port},set port(b){!this._isInvalid&&this._isRelative&&g.call(this,b,"port")},get pathname(){return this._isInvalid?"":this._isRelative?"/"+this._path.join("/"):this._schemeData},set pathname(b){!this._isInvalid&&this._isRelative&&(this._path=[],g.call(this,b,"relative path start"))},get search(){return this._isInvalid||!this._query||"?"===this._query?"":this._query},set search(b){!this._isInvalid&&this._isRelative&&(this._query=
"?","?"===b[0]&&(b=b.slice(1)),g.call(this,b,"query"))},get hash(){return this._isInvalid||!this._fragment||"#"===this._fragment?"":this._fragment},set hash(b){this._isInvalid||(this._fragment="#","#"===b[0]&&(b=b.slice(1)),g.call(this,b,"fragment"))},get origin(){var b;if(this._isInvalid||!this._scheme)return"";switch(this._scheme){case "data":case "file":case "javascript":case "mailto":return"null";case "blob":try{return(new p(this._schemeData)).origin||"null"}catch(f){}return"null"}return(b=this.host)?
this._scheme+"://"+b:""}};var q=globalScope.URL;q&&(p.createObjectURL=function(b){return q.createObjectURL.apply(q,arguments)},p.revokeObjectURL=function(b){q.revokeObjectURL(b)});globalScope.URL=p}})()};