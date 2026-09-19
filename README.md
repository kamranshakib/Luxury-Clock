<p align="center">
  <img src="icon.ico" alt="Clock Icon" width="128"/>
</p>

<h1 align="center">Luxury Clock </h1>

A beautiful and minimalist desktop clock for Windows, built with WPF and C#. This clock features a transparent and borderless design, sitting gracefully on your desktop as an elegant widget.

## Features

* **Beautiful & Transparent Design:** A completely invisible background with a soft drop shadow on the numbers for excellent readability on any wallpaper.
* **Borderless & Draggable:** You can easily move the clock anywhere on your desktop by clicking and dragging (Drag & Drop) anywhere on the window.
* **Resizable:** Hovering the mouse over the clock reveals a subtle resize grip in the bottom-right corner, allowing you to scale the clock to your preferred size.
* **Always on Bottom:** This clock is specifically programmed to always stay behind other windows so it never interferes with your active workspace.
* **Auto-Start with Windows:** The application automatically adds itself to Windows Startup, ensuring the clock is ready every time you boot your system.
* **Custom Setup Wizard:** Includes a custom-built graphical installer for easy installation, which automatically creates shortcuts on your desktop and start menu.

## Technologies Used

* **Framework:** Windows Presentation Foundation (WPF) (.NET)
* **Language:** C# and XAML
* **UI Structure:** Uses `Viewbox` for flawless and automatic scaling of the text.
* **System Integration:** Utilizes `user32.dll` functions (like `SetWindowPos`) to manage the window's position and Z-order in Windows.

## Installation & Usage

1. **Download:** My friend, go to the right side of this page under the **"Releases"** section and download the latest setup file.
2. Run the downloaded setup installation file.
3. Follow the steps in the Setup Wizard (you can choose the installation path and whether to create shortcuts).
4. Once the installation is complete, launch the application.
5. **Moving:** Left-click and hold anywhere on the clock, then drag it to your desired location on the desktop.
6. **Resizing:** Move your mouse over the clock area until the resize grip icon appears in the bottom-right corner, then drag it to adjust the size.
