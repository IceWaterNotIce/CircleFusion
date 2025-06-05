const cacheName = "Ice Water Not Ice-Circle Fusion-1.0";
const contentToCache = [
    "Build/b641055c1cbec3eb82d4479e86e21074.loader.js",
    "Build/8b217a5d8d7b643193141afc99a1bda0.framework.js",
    "Build/9e5925983dbd861a3737e66a20fd4751.data",
    "Build/564448354e867e5fbf4ab08475da2c67.wasm",
    "TemplateData/style.css"

];

self.addEventListener('install', function (e) {
    console.log('[Service Worker] Install');
    
    e.waitUntil((async function () {
      const cache = await caches.open(cacheName);
      console.log('[Service Worker] Caching all: app shell and content');
      await cache.addAll(contentToCache);
    })());
});

self.addEventListener('fetch', function (e) {
    e.respondWith((async function () {
      let response = await caches.match(e.request);
      console.log(`[Service Worker] Fetching resource: ${e.request.url}`);
      if (response) { return response; }

      response = await fetch(e.request);
      const cache = await caches.open(cacheName);
      console.log(`[Service Worker] Caching new resource: ${e.request.url}`);
      cache.put(e.request, response.clone());
      return response;
    })());
});
