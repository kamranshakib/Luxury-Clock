const { Jimp } = require('jimp');
const pngToIco = require('png-to-ico');
const fs = require('fs');
const path = require('path');

async function buildIcon() {
  const iconPath = path.join(__dirname, 'assets', 'icon.png');
  const tempPng = path.join(__dirname, 'assets', 'icon_temp.png');
  const icoPath = path.join(__dirname, 'assets', 'icon.ico');
  
  try {
    console.log('Reading generated image...');
    const image = await Jimp.read(iconPath);
    
    // Resize to a good size for ico and save as real PNG
    console.log('Converting to PNG...');
    image.resize({ w: 256, h: 256 });
    await image.write(tempPng);
    
    // Convert to ICO
    console.log('Converting to ICO...');
    const convertPng = pngToIco.default || pngToIco;
    const buf = await convertPng(tempPng);
    fs.writeFileSync(icoPath, buf);
    
    // Clean up
    fs.unlinkSync(tempPng);
    // Let's also overwrite the jpg named icon.png with a real png just in case electron builder needs it
    image.resize({ w: 512, h: 512 });
    await image.write(iconPath);
    
    console.log('Icon successfully generated at', icoPath);
  } catch (error) {
    console.error('Error generating icon:', error);
  }
}

buildIcon();
