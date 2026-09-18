const electronInstaller = require('electron-winstaller');
const path = require('path');

async function build() {
  console.log('Building Windows Installer (Setup.exe)...');
  try {
    await electronInstaller.createWindowsInstaller({
      appDirectory: path.join(__dirname, 'dist', 'luxury-clock-win32-x64'),
      outputDirectory: path.join(__dirname, 'dist', 'installer'),
      authors: 'Luxury Clock',
      exe: 'luxury-clock.exe',
      description: 'A premium modern desktop clock.',
      setupExe: 'LuxuryClockSetup.exe',
      noMsi: true,
      version: '1.0.0'
    });
    console.log('Successfully created installer at dist/installer/LuxuryClockSetup.exe');
  } catch (e) {
    console.log(`Installer build failed: ${e.message}`);
  }
}

build();
