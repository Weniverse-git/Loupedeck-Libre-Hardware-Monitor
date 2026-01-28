# Loupedeck Libre Hardware Monitor Plugin

<a href="https://buymeacoffee.com/Weniverse"><img src="https://cdn.buymeacoffee.com/buttons/v2/default-yellow.png" alt="Buy Me A Coffee" width="200"></a>

**Loupedeck / Razer Stream Controller plugin that displays real-time hardware sensor data** from [Libre Hardware Monitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor).

![Plugin Preview](docs/images/preview.png)

<br>

*This is my first vibe coding project. I've been using Loupedeck for a very long time, and the time has finally come.*

<br>

⚠️I'm making it public because I think many people are looking for this functionality. But basically, it is for personal use. I cannot guarantee ongoing maintenance.

<br>

## Features

### 29 Dynamic Actions across 3 View Types

**Text View** — sensor value displayed as text

| Action | Description |
|--------|-------------|
| CPU Temp | CPU core temperature |
| GPU Temp | GPU core temperature |
| RAM Temp | RAM (DIMM) temperature |
| CPU Load | CPU total load % |
| GPU Load | GPU core load % |
| GPU VRAM | VRAM usage (GB) |
| GPU VRAM % | VRAM usage % |
| RAM Usage | RAM usage (GB/Total) |
| CPU Power | CPU package power (W) |
| GPU Power | GPU power (W) |
| CPGPU Power | CPU + GPU combined power (W) |
| NVMe #0–#4 Temp | NVMe drive temperatures |

<br>

**Block Graph View** — percentage shown as filled blocks

| Action | Description |
|--------|-------------|
| CPU Load (Block) | 5x4 block graph |
| GPU Load (Block) | 5x4 block graph |
| GPU VRAM (Block) | 5x4 block graph |
| RAM Load (Block) | 5x4 block graph |
| Total Load (Block) | 4-row combined: CPU, GPU, VRAM, RAM |

- Each block represents 5% (except Total Load)
- Each block represents 20% (only Total Load)

<br>

**Arc Gauge View** — temperature shown as colored arc

| Action | Description |
|--------|-------------|
| CPU Temp (Gauge) | Arc gauge with color thresholds |
| GPU Temp (Gauge) | Arc gauge with color thresholds |
| RAM Temp (Gauge) | Arc gauge with color thresholds |
| NVMe #0–#4 (Gauge) | Arc gauge per NVMe drive |

<br>

## Color Thresholds

### Temperature

Text View and Gauge View share the same per-component thresholds.

| Component | Green | Yellow | Red |
|-----------|-------|--------|-----|
| CPU | < 70°C | 70–89°C | ≥ 90°C |
| GPU | < 60°C | 60–74°C | ≥ 75°C |
| NVMe | < 60°C | 60–74°C | ≥ 75°C |
| RAM | < 45°C | 45–54°C | ≥ 55°C |

### Power

| Component | Green | Yellow | Red |
|-----------|-------|--------|-----|
| CPU Power | < 90W | 90–139W | ≥ 140W |
| GPU Power | < 300W | 300–499W | ≥ 500W |
| CPGPU Power | < 390W | 390–639W | ≥ 640W |

<br>

## Requirements

- **[Libre Hardware Monitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor)** **v0.9.5+** — must be running in the system tray with enabled on port **8085**
- **Loupedeck** or **Razer Stream Controller** with Loupedeck software (tested on **v6.2.4.228**)

<br>

## Installation

0. Download `Libre Hardware Monitor` and run as administrator
1. Download `LLHMPlugin.lplug4` from the [Releases](https://github.com/Weniverse-git/Loupedeck-Libre-Hardware-Monitor/releases) page
2. Double-click the downloaded file
3. Open Loupedeck - Show and Hide Plugins - Settings - install plugin from file - select `LLHMPlugin.lplug4`
4. Show plugin Hardware Monitor
5. Find actions under the **Hardware Monitor** category
6. Drag lists as you want

![Installation](https://github.com/user-attachments/assets/27bd3657-cf12-4d61-812a-40038723ca31)

<br>

### Troubleshooting

**Enabling LHM HTTP Server**
1. Open Libre Hardware Monitor
2. Go to **Options > Remote Web Server**
3. Set port to **8085**

![Libre Hardware Monitor Settings](https://github.com/user-attachments/assets/825bfc6a-638d-42b3-b448-1c08955a0f8d)

**The administrator mode and autostart of LHM is broken somewhere.** The most reliable method is to register LHM Application on the Loupedeck, launch it with a button, and then approve the administrator mode to turn it on.

<br>

## Sensor Configuration

This plugin reads sensors from LHM's HTTP API. Default sensor paths are configured for:

| Component | Sensor Path |
|-----------|-------------|
| CPU | `/amdcpu/0/` |
| GPU | `/gpu-nvidia/0/` |
| RAM | `/ram/` |
| NVMe | `/nvme/0/` ~ `/nvme/4/` |

> **Note:** Sensor paths may differ depending on your hardware. Check `http://localhost:8085/data.json` for your system's sensor IDs.

<br>

## License

[MIT](https://opensource.org/licenses/MIT) — Copyright (c) 2026 Weniverse

<a href="https://buymeacoffee.com/Weniverse"><img src="https://cdn.buymeacoffee.com/buttons/v2/default-yellow.png" alt="Buy Me A Coffee" width="200"></a>
