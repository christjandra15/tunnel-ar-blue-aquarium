# Tunnel AR — Blue Aquarium Project

**Christian Nathaniel Tjandra** · [github.com/christjandra15](https://github.com/christjandra15)

---

## Overview

This repository contains source excerpts from the **Living Ocean Tunnel Replica**,
a web-based augmented reality exhibit developed as part of the **Blue Aquarium
Project**, a collaboration between **Aquaria KLCC** (Kuala Lumpur's public
aquarium) and **Asia Pacific University (APU)**.

The exhibit lets visitors scan a QR code to open a browser-based AR experience —
no app install required — place a life-size replica of Aquaria KLCC's acrylic
Living Ocean tunnel in their physical surroundings, and walk through it. A
custom shader reproduces the real optical magnification effect caused by the
tunnel's curved acrylic glass, so fish viewed through the glass appear
distorted the same way they do in the physical exhibit — turning a subtle
physics phenomenon into something visitors can directly observe and compare.

Built in **Unity 6 (URP)** using **Imagine WebAR's World Tracker SDK** for
markerless 6DOF tracking, targeting mobile browsers via WebGL.

---

## What's here

This is a curated set of source excerpts, not a full project export. Third-party
SDK code, marketplace assets, and generated build artifacts are intentionally
excluded.

### `Scripts/Gameplay/`
- **`TunnelVisibilityController.cs`** — Toggles the visibility of fish/marine
  life renderers based on the AR camera's distance to the tunnel origin. When
  the visitor's device is inside the tunnel's trigger radius, exterior marine
  life is hidden to avoid render clutter and depth-sorting artifacts between
  the tunnel's transparent glass and the animals swimming around it.

### `Scripts/UI/`
- **`InfoUIController.cs`** — Drives the exhibit's informational overlay panel:
  binds a UI button to toggle a hidden-by-default info panel, used to surface
  educational content about the tunnel and its marine life without cluttering
  the default AR view.

### `Scripts/Shaders/`
- **`UI_AlwaysOnTop.shader`** — A custom unlit UI shader (`ZTest Always`,
  `ZWrite Off`) used to keep specific UI graphics (e.g. the Aquaria KLCC logo)
  visibly in front of 3D scene content when rendered through a
  Screen-Space-Camera canvas, rather than the default Screen-Space-Overlay
  mode. This was needed because the SDK's screenshot capture function renders
  only through the AR camera, and Overlay-mode UI is invisible to camera-based
  rendering — see Technical Highlights below.
- **`MagnificationShader-NodeGraph.md`** — Documented structure of the
  project's Shader Graph asset that produces the tunnel glass's optical
  magnification effect, since Shader Graph is serialized as JSON and isn't
  meaningful to read as raw source.

### `Scripts/WebGL-Template/`
- **`index.html`** — The production Unity WebGL template, patched with a
  device-compatibility fix for a motion-sensor permission bug encountered
  during real-device testing at Aquaria KLCC — see Technical Highlights below.

---

## Technical Highlights

### Diagnosing a silent AR tracking failure on Xiaomi/HyperOS devices

During staff testing at Aquaria KLCC, the experience worked correctly on most
Android phones but silently failed to track after placement on several
Xiaomi/POCO devices (HyperOS) — the floor-scan indicator appeared and
placement succeeded, but the tracked object never updated position afterward,
with no error shown to the visitor.

**Diagnosis:** Rather than guessing at settings, I remote-debugged an affected
device directly via `chrome://inspect`, and cross-referenced the console
output against the SDK's own (minified, third-party) JavaScript source. This
traced the failure to a specific, reproducible cause: on devices where the OS
blocks motion-sensor access at the system level, `new AbsoluteOrientationSensor()`
throws a `SecurityError` **synchronously** — outside the SDK's own `Promise`
chain — so its own `.catch()` handler never runs, and the failure is silently
swallowed instead of surfacing to the visitor.

**Fix:** Rather than patching the vendor's obfuscated SDK file directly (
unsupported and would be overwritten on the next SDK update), I added a
pre-flight check in the WebGL template's own `index.html`, run before the
SDK's sensor initialization:

1. Attempt to construct `AbsoluteOrientationSensor` in a `try/catch` first.
2. If construction fails, remove the API from `window` entirely — this causes
   the SDK's own existing fallback logic to use the older, more permissive
   `DeviceOrientationEvent` API instead, resolving the issue with **no action
   required from the visitor** on most affected devices.
3. If neither sensor API is available at all, show a clear on-screen message
   with device-specific remediation steps and a one-tap retry button, instead
   of a silent failure.

This is included in this repo (`Scripts/WebGL-Template/index.html`) as an
example of tracing a platform-specific production bug through remote
debugging and third-party source inspection, then shipping a fix that
degrades gracefully rather than papering over the symptom.

### Screenshot capture excluding UI (Screen Space rendering modes)

The SDK's built-in screenshot feature renders the scene by manually invoking
`Camera.main.Render()` into a texture. This correctly captures 3D content, but
UI Canvases set to Unity's default **Screen Space — Overlay** mode render
directly to the screen as a separate pass, entirely outside any camera's
render output — so overlay UI (including exhibit branding) was silently
missing from every screenshot visitors took.

Fixed by moving the affected UI elements to a dedicated **Screen Space —
Camera** canvas bound to the AR camera (so it participates in that camera's
render output and is captured correctly), paired with a custom
`ZTest Always` shader (`UI_AlwaysOnTop.shader`, included above) so the
UI still renders correctly in front of the 3D tunnel mesh, which it would
otherwise be occluded by once made part of normal depth-tested 3D rendering.

---

## Setup

These files are excerpts for portfolio/technical reference and are **not a
runnable standalone project** — they depend on the full Unity project
(scenes, prefabs, 3D models, and the licensed Imagine WebAR SDK), which is not
included here.

If reviewing in context:

- **Engine:** Unity 6000.3.x (Unity 6 LTS)
- **Render Pipeline:** Universal Render Pipeline (URP)
- **AR SDK:** Imagine WebAR — World Tracker (markerless 6DOF, WebGL)
- **Target:** WebGL build, mobile browser (Chrome/Android, Safari/iOS)
- **Shader authoring:** Shader Graph (URP Lit target) + hand-written HLSL

---

## License

See [`LICENSE`](./LICENSE). This code is shared for portfolio and educational
reference only.
