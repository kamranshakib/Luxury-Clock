setInterval(async () => {
  const { activeWindow } = await import('active-win');
  const win = await activeWindow();
  console.log(win);
}, 2000);
