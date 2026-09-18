const { BrowserWindow } = require('electron');
const path = require('path');

function createWindow(store) {
  const bounds = store.get('windowBounds');
  const alwaysOnTop = store.get('alwaysOnTop');

  const mainWindow = new BrowserWindow({
    width: bounds.width || 400,
    height: bounds.height || 250,
    x: bounds.x,
    y: bounds.y,
    icon: path.join(__dirname, '..', '..', 'assets', 'icon.ico'),
    transparent: true,
    frame: false,
    hasShadow: false,
    alwaysOnTop: alwaysOnTop,
    resizable: true,
    minWidth: 200,
    minHeight: 150,
    skipTaskbar: true, // Don't show in taskbar, it's a desktop widget
    webPreferences: {
      nodeIntegration: false,
      contextIsolation: true,
      preload: path.join(__dirname, '..', 'preload', 'preload.js')
    }
  });

  mainWindow.loadFile(path.join(__dirname, '..', 'renderer', 'index.html'));

  return mainWindow;
}

function setupWindow(mainWindow, store) {
  // Save window bounds when moved or resized
  const saveBounds = () => {
    store.set('windowBounds', mainWindow.getBounds());
  };

  mainWindow.on('resized', saveBounds);
  mainWindow.on('moved', saveBounds);

  // When window closes, clean up reference
  mainWindow.on('closed', () => {
    mainWindow = null;
  });
}

module.exports = {
  createWindow,
  setupWindow
};
