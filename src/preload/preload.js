const { contextBridge, ipcRenderer } = require('electron');

contextBridge.exposeInMainWorld('electronAPI', {
  getSettings: () => ipcRenderer.invoke('get-settings'),
  setSetting: (key, value) => ipcRenderer.send('set-setting', key, value),
  resizeWindow: (width, height) => ipcRenderer.send('resize-window', width, height),
  resetSettings: (defaults) => ipcRenderer.send('reset-settings', defaults),
  onSettingsReset: (callback) => ipcRenderer.on('settings-reset', (_event, value) => callback(value)),
  onSettingUpdated: (callback) => ipcRenderer.on('setting-updated', (_event, key, value) => callback(key, value)),
  onOpenSettings: (callback) => ipcRenderer.on('open-settings', () => callback()),
  onOsThemeUpdated: (callback) => ipcRenderer.on('os-theme-updated', (_event, isDark) => callback(isDark)),
  exitApp: () => ipcRenderer.send('exit-app')
});
