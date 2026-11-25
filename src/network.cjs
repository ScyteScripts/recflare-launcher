'use strict';
const product = require('../config/product.json');
function rewriteUrl(value) {
  let url;
  try { url = new URL(value); } catch { return value; }
  if (!['https:', 'http:'].includes(url.protocol)) return value;
  const host = url.hostname.toLowerCase();
  for (const domain of ['recroom.com', 'rec.net', 'recroom.net', 'recroom.cloud']) {
    if (host !== domain && !host.endsWith('.' + domain)) continue;
    const prefix = host === domain ? '' : host.slice(0, -(domain.length + 1));
    url.hostname = (prefix ? prefix + '.' : '') + new URL(product.webOrigin).hostname;
    url.protocol = 'https:';
    url.port = '';
    return url.href;
  }
  return value;
}
function isOriginalTelemetry(value) {
  try { const host = new URL(value).hostname; return host === 'recroom-dataplane.rudderstack.com' || host === 'statsigapi.net' || host.endsWith('.statsigapi.net'); }
  catch { return false; }
}
function attach(electron) {
  electron.app.whenReady().then(() => {
    electron.session.defaultSession.webRequest.onBeforeRequest((details, callback) => {
      if (isOriginalTelemetry(details.url)) return callback({cancel:true});
      const rewritten = rewriteUrl(details.url);
      callback(rewritten === details.url ? {} : {redirectURL:rewritten});
    });
  });
}
module.exports = {rewriteUrl,isOriginalTelemetry,attach};
