# Luxury Clock

A premium, modern desktop clock application built with Electron.

## Installation

Windows users can download the installer from the `release/` directory (once built) and follow the installation wizard:
`Luxury-Clock-Setup.exe`

## Build from source

To run the application in development mode:
```bash
npm install
npm start
```

## Build Windows installer

To build the professional Windows installer (`.exe`), you must run the following command. 
**Important for Windows:** You must run this command in an Administrator terminal (Run as Administrator) to allow the build tool to create necessary symbolic links.

```bash
npm run build:win
```

The compiled installer will appear inside the `release/` folder.
