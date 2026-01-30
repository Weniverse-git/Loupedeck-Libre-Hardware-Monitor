# Loupedeck Libre Hardware Monitor Plugin
**Loupedeck / Razer Stream Controller plugin** that displays real-time hardware sensor data from [Libre Hardware Monitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor).

![Operating](https://github.com/user-attachments/assets/1dae7f00-3889-487f-b5fd-08695a2eb5ce)

<br>

*This is my first vibe coding project. I've been using Loupedeck for a very long time, and the time has finally come.*

<br>

## Features

![Plugin Preview](docs/images/preview.png)

*Currently tested on AMD CPUs / Nvidia GPUs only*
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

**LHM keeps crashing (NullReferenceException in DiskInfoToolkit)**

If LHM crashes repeatedly with an error like this in Windows Event Viewer:
```
System.NullReferenceException
   at DiskInfoToolkit.Storage.IdentifyStorageController()
   at DiskInfoToolkit.StorageManager.HandleUnpartitionedDrive()
   at DiskInfoToolkit.StorageManager.DevicesChangedListener()
```

This is a **known bug in LHM v0.9.5** ([DiskInfoToolkit Issue #6](https://github.com/Blacktempel/DiskInfoToolkit/issues/6)). It occurs when handling unpartitioned drives or drives with bad sectors.

**Solution:** Download the latest build from [GitHub Actions](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor/actions) and replace the files in your LHM installation folder:
1. Go to the [latest successful build](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor/actions/workflows/build.yml?query=branch%3Amaster+is%3Asuccess)
2. Download the `LibreHardwareMonitor` artifact (GitHub login required)
3. Close LHM completely
4. Extract and overwrite files in `C:\Program Files\Libre Hardware Monitor\`
5. Restart LHM

<br>

## Sensor Configuration

This plugin **automatically detects** your CPU and GPU from LHM's HTTP API.

### Supported Hardware (v1.2.0+)

| Component | Supported Vendors |
|-----------|-------------------|
| CPU | AMD, Intel |
| GPU | NVIDIA, AMD (Radeon), Intel |
| RAM | All (fixed path) |
| NVMe | `/nvme/0/` ~ `/nvme/4/` |

> **Note:** The plugin auto-detects your hardware at startup. No manual configuration needed. Check `http://localhost:8085/data.json` to verify LHM is running.

<br>

## License

[MIT](https://opensource.org/licenses/MIT) — Copyright (c) 2026 Weniverse

<br>

## Comment

If you find my project useful to you, please consider giving a star ⭐

<br>

<a href="https://buymeacoffee.com/Weniverse"><img src="https://cdn.buymeacoffee.com/buttons/v2/default-yellow.png" alt="Buy Me A Coffee" width="200"></a>
