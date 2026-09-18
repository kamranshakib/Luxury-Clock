const { app } = require('electron');
const { createWindow, setupWindow } = require('./window');
const { createTray } = require('./tray');
const { setupIpcHandlers } = require('./ipcHandlers');
const Store = require('./store');

// Disable remote module and node integration globally as best practice
app.on('web-contents-created', (event, contents) => {
  contents.on('will-navigate', (event, navigationUrl) => {
    event.preventDefault();
  });
  contents.setWindowOpenHandler(() => {
    return { action: 'deny' };
  });
});

const store = new Store({
  configName: 'user-preferences',
  defaults: {
    use24Hour: false,
    showSeconds: true,
    showDate: true,
    showDay: true,
    clockSize: 1, // Scale factor
    opacity: 1.0,
    glassBlur: 10,
    theme: 'auto',
    alwaysOnTop: true,
    startWithWindows: false,
    animationsEnabled: true,
    windowBounds: { width: 400, height: 250 }
  }
});

let mainWindow;
let tray;

app.whenReady().then(() => {
  // Set autostart
  if (app.isPackaged) {
    app.setLoginItemSettings({
      openAtLogin: store.get('startWithWindows')
    });
  }

  setupIpcHandlers(store);
  
  mainWindow = createWindow(store);
  setupWindow(mainWindow, store);

  tray = createTray(mainWindow, store);

  app.on('activate', () => {
    if (mainWindow === null) {
      mainWindow = createWindow(store);
      setupWindow(mainWindow, store);
    }
  });
});

app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') {
    app.quit();
  }
});
