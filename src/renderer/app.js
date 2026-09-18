// DOM Elements
const timeMainEl = document.getElementById('time-main');
const timeSecondsEl = document.getElementById('time-seconds');
const rootElement = document.documentElement;
const clockInterface = document.getElementById('clock-interface');
const settingsTrigger = document.getElementById('settings-trigger');
const settingsPanel = document.getElementById('settings-panel');

// Global State
let config = {};

// Initialize application
async function initApp() {
  // Get initial settings
  config = await window.electronAPI.getSettings();
  applySettings(config);
  
  // Start clock loop
  requestAnimationFrame(updateClock);
  
  // Listen for setting updates from Main (IPC)
  window.electronAPI.onSettingUpdated((key, value) => {
    config[key] = value;
    applySetting(key, value);
  });
  
  // Listen for full reset
  window.electronAPI.onSettingsReset((newSettings) => {
    config = newSettings;
    applySettings(config);
  });

  // Listen for open-settings from Tray
  window.electronAPI.onOpenSettings(() => {
    openSettings();
  });

  // Listen for OS theme updates
  if (window.electronAPI.onOsThemeUpdated) {
    window.electronAPI.onOsThemeUpdated((isDark) => {
      config.osDark = isDark;
      if (config.theme === 'auto') {
        applySetting('theme', 'auto');
      }
    });
  }
}

// Clock Loop
let lastSecond = -1;
function updateClock() {
  const now = new Date();
  const second = now.getSeconds();

  // Only update DOM if the second has changed (optimizes performance)
  if (second !== lastSecond) {
    lastSecond = second;
    
    // Time
    let hours = now.getHours();
    const minutes = String(now.getMinutes()).padStart(2, '0');
    const secondsStr = String(second).padStart(2, '0');
    
    if (!config.use24Hour) {
      hours = hours % 12 || 12;
    }
    const hoursStr = String(hours).padStart(2, '0');
    
    timeMainEl.textContent = `${hoursStr}:${minutes}`;
    
    if (config.showSeconds) {
      timeSecondsEl.textContent = secondsStr;
    }
  }
  
  // Use a timeout to run near the next second boundary instead of 60fps for battery savings
  const msToNextSecond = 1000 - now.getMilliseconds();
  setTimeout(() => requestAnimationFrame(updateClock), msToNextSecond);
}

// Apply all settings initially
function applySettings(settings) {
  Object.keys(settings).forEach(key => applySetting(key, settings[key]));
}

// Apply an individual setting
function applySetting(key, value) {
  switch (key) {
    case 'showSeconds':
      timeSecondsEl.style.display = value ? 'inline' : 'none';
      break;
    case 'clockSize':
      rootElement.style.setProperty('--clock-scale', value);
      break;
    case 'opacity':
      rootElement.style.setProperty('--opacity', value);
      break;
    case 'glassBlur':
      rootElement.style.setProperty('--blur', `${value}px`);
      break;
    case 'theme':
      if (value === 'auto') {
        // If config.osDark is undefined, default to true (dark) to be safe
        const isDark = config.osDark !== undefined ? config.osDark : true;
        rootElement.setAttribute('data-theme', isDark ? 'minimal' : 'minimal-light');
      } else {
        rootElement.setAttribute('data-theme', value);
      }
      break;
    case 'animationsEnabled':
      if (value) {
        clockInterface.classList.add('animated');
      } else {
        clockInterface.classList.remove('animated');
      }
      break;
    // use24Hour is handled in updateClock
    // windowBounds, alwaysOnTop, startWithWindows are handled in main process
  }
}

// Dynamic Window Sizing
function resizeToClock() {
  if (settingsPanel.classList.contains('open')) return;
  const rect = clockInterface.getBoundingClientRect();
  if (window.electronAPI.resizeWindow) {
    // Add small buffer to prevent scrollbars or rounding issues
    window.electronAPI.resizeWindow(Math.ceil(rect.width) + 4, Math.ceil(rect.height) + 4);
  }
}

const resizeObserver = new ResizeObserver(() => {
  resizeToClock();
});
resizeObserver.observe(clockInterface);

// Settings Panel Toggle
function openSettings() {
  if (window.electronAPI.resizeWindow) {
    window.electronAPI.resizeWindow(380, 520); // Settings panel fixed size
  }
  settingsPanel.classList.add('open');
  // Dispatch a custom event so settings.js knows to sync UI state
  window.dispatchEvent(new CustomEvent('settings-opened', { detail: config }));
}

// Allow settings.js to trigger resize back
window.addEventListener('settings-closed', () => {
  resizeToClock();
});

settingsTrigger.addEventListener('click', openSettings);

// Boot
initApp();
