const express = require('express');
const wol = require('wol');
const ping = require('ping');

const app = express();
const PORT = 5000;

app.use(express.static(__dirname));

const DEVICES = {
  pc1: { mac: 'AA:BB:CC:DD:EE:01', name: 'Office PC', host: 'OFFICE-PC' },
  pc2: { mac: 'AA:BB:CC:DD:EE:02', name: 'Living Room PC', host: 'LIVINGROOM' },
  pc3: { mac: 'AA:BB:CC:DD:EE:03', name: 'Server', host: 'SERVER' },
  pc4: { mac: 'AA:BB:CC:DD:EE:04', name: 'Test Machine', host: 'TEST' },
  pc5: { mac: 'AA:BB:CC:DD:EE:05', name: 'Laptop', host: 'LAPTOP' },
  pc6: { mac: 'AA:BB:CC:DD:EE:06', name: 'Media Center', host: 'MEDIA' },
};

app.get('/', (req, res) => {
  let deviceCards = '';
  for (const [id, device] of Object.entries(DEVICES)) {
    deviceCards += `
      <div class="card">
        <h2>${device.name}</h2>
        <div id="status-${id}" class="status-circle"></div>
        <form action="/wake/${id}">
          <button type="submit">Wake Up</button>
        </form>
      </div>
    `;
  }

  res.send(`
  <!DOCTYPE html>
  <html lang="en">
  <head>
    <meta charset="UTF-8">
    <title>WakeOnLAN</title>
    <style>
      body {
        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        background: #f4f6f9;
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        height: 100vh;
        margin: 0;
      }
      h1 { margin-bottom: 30px; color: #333; }
      .container { display: flex; gap: 20px; flex-wrap: wrap; justify-content:center; }
      .card {
        background: white;
        border-radius: 12px;
        box-shadow: 0 4px 12px rgba(0,0,0,0.1);
        padding: 30px;
        text-align: center;
        transition: transform 0.2s ease;
      }
      .card:hover { transform: translateY(-5px); }
      button {
        background: #0078D7;
        color: white;
        border: none;
        border-radius: 8px;
        font-size: 18px;
        padding: 12px 25px;
        cursor: pointer;
        transition: background 0.2s ease;
      }
      button:hover { background: #005a9e; }
      .status-circle {
        width: 20px;
        height: 20px;
        border-radius: 50%;
        margin: 10px auto;
        background: gray;
        box-shadow: 0 0 8px rgba(0,0,0,0.3);
        transition: background 0.3s, box-shadow 0.3s;
      }
      .status-circle.online {
        background: #4CAF50;
        box-shadow: 0 0 15px #4CAF50, 0 0 25px #4CAF50;
      }
      .status-circle.offline {
        background: #E53935;
        box-shadow: 0 0 15px #E53935, 0 0 25px #E53935;
      }
    </style>
  </head>
  <body>
    <img src="STIRIXIS.png" alt="Stirixis Logo" style="width:250px; margin-bottom:20px;">
    <div class="container">
      ${deviceCards}
    </div>
    <button onclick="checkStatuses()">Check Status</button>

    <script>
      async function checkStatuses() {
        try {
          const response = await fetch('/status');
          const statuses = await response.json();
          for (const [id, state] of Object.entries(statuses)) {
            const circle = document.getElementById('status-' + id);
            if (circle) {
              circle.classList.remove('online', 'offline');
              circle.classList.add(state === 'online' ? 'online' : 'offline');
            }
          }
        } catch (err) {
          console.error('Error checking statuses:', err);
        }
      }
    </script>
  </body>
  </html>
  `);
});

app.get('/status', async (req, res) => {
  const statuses = {};
  for (const [id, device] of Object.entries(DEVICES)) {
    try {
      const result = await ping.promise.probe(device.host, { timeout: 2 });
      statuses[id] = result.alive ? 'online' : 'offline';
    } catch {
      statuses[id] = 'offline';
    }
  }
  res.json(statuses);
});

app.get('/wake/:id', async (req, res) => {
  const device = DEVICES[req.params.id];
  if (!device) return res.send('Unknown device');

  try {
    const result = await ping.promise.probe(device.host, { timeout: 2 });
    let message = result.alive ? `⚠️ ${device.name} is already ON.` : `✅ Sending wake packet to ${device.name}...`;

    wol.wake(device.mac, { address: '10.0.0.255' }, (err) => {
      if (err) message += ` ⚠️ Error sending WOL packet: ${err}`;
      else if (!result.alive) message = `✅ ${device.name} powers on!`;

      res.send(`
        <html>
          <head>
            <style>
              body {
                font-family: Arial, sans-serif;
                background: #f4f6f9;
                display: flex;
                align-items: center;
                justify-content: center;
                height: 100vh;
                margin: 0;
              }
              .modal {
                background: white;
                padding: 30px;
                border-radius: 12px;
                box-shadow: 0 4px 12px rgba(0,0,0,0.2);
                text-align: center;
                max-width: 400px;
              }
              h2 { margin-bottom: 20px; }
              button {
                background: #0078D7;
                color: white;
                border: none;
                border-radius: 8px;
                font-size: 16px;
                padding: 10px 20px;
                cursor: pointer;
                margin-top: 20px;
              }
              button:hover {
                background: #005a9e;
              }
            </style>
          </head>
          <body>
            <div class="modal">
              <h2>${message}</h2>
              <form action="/"><button type="submit">OK</button></form>
            </div>
          </body>
        </html>
      `);
    });
  } catch (err) {
    res.send(`<h2>Error checking status: ${err}</h2>`);
  }
});

app.listen(PORT, () => {
  console.log(`🚀 WOL app running at http://localhost:${PORT}`);
});
