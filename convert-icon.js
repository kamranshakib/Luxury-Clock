const { Jimp } = require('jimp');
const fs = require('fs');
const pngToIco = require('png-to-ico');

async function processIcon() {
  try {
    const imagePath = 'C:\\Users\\Kamran Shakib\\.gemini\\antigravity-ide\\brain\\e4f4018e-9500-43e7-8eec-5995faba95a8\\luxury_clock_icon_1789688171826.jpg';
    
    // Convert JPG to PNG
    const image = await Jimp.read(imagePath);
    image.resize({ w: 256, h: 256 });
    await image.write('assets/icon.png');
    
    // Convert PNG to ICO
    const convertPng = pngToIco.default || pngToIco;
    const buf = await convertPng('assets/icon.png');
    fs.writeFileSync('assets/icon.ico', buf);
    console.log('Successfully created assets/icon.ico');
  } catch (e) {
    console.error(e);
  }
}
processIcon();
