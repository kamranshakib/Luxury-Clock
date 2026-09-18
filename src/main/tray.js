const { Tray, Menu, app, nativeImage } = require('electron');
const path = require('path');

function createTray(mainWindow, store) {
  // Use a placeholder icon for now, ideally an .ico for windows
  const iconPath = path.join(__dirname, '..', '..', 'assets', 'icon.ico');
  
  let trayIcon;
  try {
    trayIcon = nativeImage.createFromPath(iconPath);
  } catch(e) {
    // Fallback if icon isn't there yet
    trayIcon = nativeImage.createEmpty();
  }

  const tray = new Tray(trayIcon);
  tray.setToolTip('Luxury Clock');

  const updateMenu = () => {
    const isAlwaysOnTop = store.get('alwaysOnTop');
    const startWithWindows = store.get('startWithWindows');
    
    const contextMenu = Menu.buildFromTemplate([
      { label: 'Luxury Clock', enabled: false },
      { type: 'separator' },
      {
        label: 'Show Clock',
        click: () => {
          if (mainWindow) {
            mainWindow.show();
          }
        }
      },
      {
        label: 'Hide Clock',
        click: () => {
          if (mainWindow) {
            mainWindow.hide();
          }
        }
      },
      { type: 'separator' },
      {
        label: 'Settings...',
        click: () => {
          if (mainWindow) {
            mainWindow.show();
            mainWindow.webContents.send('open-settings');
          }
        }
      },
      { type: 'separator' },
      {
        label: 'Always on Top',
        type: 'checkbox',
        checked: isAlwaysOnTop,
        click: (item) => {
          store.set('alwaysOnTop', item.checked);
          if (mainWindow) {
            mainWindow.setAlwaysOnTop(item.checked);
          }
        }
      },
      {
        label: 'Start with Windows',
        type: 'checkbox',
        checked: startWithWindows,
        click: (item) => {
          store.set('startWithWindows', item.checked);
          if (app.isPackaged) {
            app.setLoginItemSettings({
              openAtLogin: item.checked
            });
          }
        }
      },
      { type: 'separator' },
      {
        label: 'Exit',
        click: () => {
          app.isQuitting = true;
          app.quit();
        }
      }
    ]);
    
    tray.setContextMenu(contextMenu);
  };

  updateMenu();

  // Handle store changes dynamically if needed (or just let UI update it)
  tray.on('double-click', () => {
    if (mainWindow) {
      if (mainWindow.isVisible()) {
        mainWindow.hide();
      } else {
        mainWindow.show();
      }
    }
  });

  return tray;
}

module.exports = { createTray };
