// Settings Panel DOM Elements
const closeBtn = document.getElementById('close-settings');
const panel = document.getElementById('settings-panel');
const resetBtn = document.getElementById('btn-reset');
const exitBtn = document.getElementById('btn-exit');

// Input Elements
const inputs = {
  use24Hour: document.getElementById('setting-24hr'),
  showSeconds: document.getElementById('setting-seconds'),
  theme: document.getElementById('setting-theme'),
  clockSize: document.getElementById('setting-size'),
  opacity: document.getElementById('setting-opacity'),
  glassBlur: document.getElementById('setting-blur'),
  alwaysOnTop: document.getElementById('setting-top'),
  startWithWindows: document.getElementById('setting-autostart'),
  animationsEnabled: document.getElementById('setting-animations')
};

// Value Displays
const displays = {
  clockSize: document.getElementById('val-size'),
  opacity: document.getElementById('val-opacity'),
  glassBlur: document.getElementById('val-blur')
};

// Close panel
closeBtn.addEventListener('click', () => {
  panel.classList.remove('open');
  window.dispatchEvent(new Event('settings-closed'));
});

// Sync UI inputs with current config when panel opens
window.addEventListener('settings-opened', (e) => {
  const cfg = e.detail;
  
  inputs.use24Hour.checked = cfg.use24Hour;
  inputs.showSeconds.checked = cfg.showSeconds;
  
  inputs.theme.value = cfg.theme;
  
  inputs.clockSize.value = cfg.clockSize;
  displays.clockSize.textContent = `${Math.round(cfg.clockSize * 100)}%`;
  
  inputs.opacity.value = cfg.opacity;
  displays.opacity.textContent = `${Math.round(cfg.opacity * 100)}%`;
  
  inputs.glassBlur.value = cfg.glassBlur;
  displays.glassBlur.textContent = `${cfg.glassBlur}px`;
  
  inputs.alwaysOnTop.checked = cfg.alwaysOnTop;
  inputs.startWithWindows.checked = cfg.startWithWindows;
  inputs.animationsEnabled.checked = cfg.animationsEnabled;
});

// Bind Input Change Events
function bindInput(key, el, isCheckbox = false) {
  el.addEventListener('change', (e) => {
    const val = isCheckbox ? e.target.checked : e.target.value;
    // Tell main process to save
    window.electronAPI.setSetting(key, val);
  });
}

function bindSlider(key, el, displayEl, formatFn) {
  el.addEventListener('input', (e) => {
    const val = parseFloat(e.target.value);
    displayEl.textContent = formatFn(val);
    window.electronAPI.setSetting(key, val);
  });
}

bindInput('use24Hour', inputs.use24Hour, true);
bindInput('showSeconds', inputs.showSeconds, true);
bindInput('alwaysOnTop', inputs.alwaysOnTop, true);
bindInput('startWithWindows', inputs.startWithWindows, true);
bindInput('animationsEnabled', inputs.animationsEnabled, true);

inputs.theme.addEventListener('change', (e) => {
  window.electronAPI.setSetting('theme', e.target.value);
});

bindSlider('clockSize', inputs.clockSize, displays.clockSize, (v) => `${Math.round(v * 100)}%`);
bindSlider('opacity', inputs.opacity, displays.opacity, (v) => `${Math.round(v * 100)}%`);
bindSlider('glassBlur', inputs.glassBlur, displays.glassBlur, (v) => `${v}px`);

// Buttons
resetBtn.addEventListener('click', () => {
  if (confirm('Reset all settings to defaults?')) {
    window.electronAPI.resetSettings({
      use24Hour: false,
      showSeconds: true,
      clockSize: 1,
      opacity: 1.0,
      glassBlur: 10,
      theme: 'auto',
      alwaysOnTop: true,
      startWithWindows: false,
      animationsEnabled: true
    });
    // Give it a brief moment then re-sync UI
    setTimeout(() => {
      window.dispatchEvent(new CustomEvent('settings-opened', { detail: config }));
    }, 100);
  }
});

exitBtn.addEventListener('click', () => {
  window.electronAPI.exitApp();
});
