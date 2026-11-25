const { contextBridge, ipcRenderer } = require('electron');

contextBridge.exposeInMainWorld('launcher', {
  windowControl: (action) => ipcRenderer.invoke('window:control', action),
  openExternal: (url) => ipcRenderer.invoke('open:external', url),
});
