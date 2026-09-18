const { ipcMain, app, nativeTheme, BrowserWindow } = require('electron');

function setupIpcHandlers(store) {
  // Get all settings initially
  ipcMain.handle('get-settings', () => {
    const settings = store.getAll();
    settings.osDark = nativeTheme.shouldUseDarkColors;
    return settings;
  });

  // Listen for OS theme changes
  nativeTheme.on('updated', () => {
    BrowserWindow.getAllWindows().forEach(win => {
      win.webContents.send('os-theme-updated', nativeTheme.shouldUseDarkColors);
    });
  });

  // Update a specific setting
  ipcMain.on('set-setting', (event, key, value) => {
    store.set(key, value);
    
    // Apply system-level settings immediately
    if (key === 'alwaysOnTop') {
      const { BrowserWindow } = require('electron');
      const win = BrowserWindow.fromWebContents(event.sender);
      if (win) {
        win.setAlwaysOnTop(value);
      }
    } else if (key === 'startWithWindows') {
      if (app.isPackaged) {
        app.setLoginItemSettings({
          openAtLogin: value
        });
      }
    }
    
    // Broadcast setting change to renderer (useful if changed from elsewhere)
    event.sender.send('setting-updated', key, value);
  });
  
  // Allow renderer to reset to defaults
  ipcMain.on('reset-settings', (event, defaults) => {
    // Only reset the visual and feature settings, not window bounds
    const currentBounds = store.get('windowBounds');
    
    Object.keys(defaults).forEach(key => {
      if (key !== 'windowBounds') {
        store.set(key, defaults[key]);
      }
    });
    
    store.set('windowBounds', currentBounds); // Restore bounds
    
    // Re-apply alwaysOnTop
    const { BrowserWindow } = require('electron');
    const win = BrowserWindow.fromWebContents(event.sender);
    if (win) {
      win.setAlwaysOnTop(defaults.alwaysOnTop);
    }
    
    if (app.isPackaged) {
        app.setLoginItemSettings({
          openAtLogin: defaults.startWithWindows
        });
    }

    // Send back all new settings
    event.sender.send('settings-reset', store.getAll());
  });

  // Resize window dynamically
  ipcMain.on('resize-window', (event, width, height) => {
    const { BrowserWindow } = require('electron');
    const win = BrowserWindow.fromWebContents(event.sender);
    if (win) {
      win.setContentSize(width, height);
    }
  });

  // Handle custom exit from renderer (e.g. from settings panel if added)
  ipcMain.on('exit-app', () => {
    app.quit();
  });
}

module.exports = { setupIpcHandlers };
