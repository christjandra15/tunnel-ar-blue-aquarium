# MagnificationShader (Shader Graph)

`Assets/Tunnel Assets/MagnificationShader.shadergraph`

This is a Unity **Shader Graph** asset (a visual node graph serialized as JSON),
not a hand-written `.shader` file, so the raw source isn't meaningful to display
here. This file documents the actual node graph and its live parameter values,
read directly from the current project.

## Purpose

Applied to the tunnel's acrylic glass mesh, this shader creates the visual
effect of fish appearing magnified/shifted when viewed through the curved
glass — replicating the real optical distortion of the physical tunnel's
acrylic wall at Aquaria KLCC.

## Node graph

```
Screen Position (Default space)
        │
        ▼
Tiling And Offset   Tiling = (1.3, 1.3)   Offset = (-0.15, -0.15)
        │
        ▼
Clamp               Min = (0.001, 0.001)   Max = (0.999, 0.999)
        │
        ▼
Scene Color   (samples _CameraOpaqueTexture at the clamped UV)
        │
        ▼
Multiply  ×  Color (tint)
        │
        ▼
Base Color  →  Fragment output (Universal Lit target, Transparent surface)
```

## Why the Clamp node exists

The `Tiling And Offset` node scales the sampled screen UVs by 1.3, which pushes
roughly the outer 11% of the sample range outside the `[0, 1]` texture bounds.
On native builds this edge case is quietly absorbed by the platform's default
texture wrap/clamp behaviour, but in WebGL builds the same out-of-bounds sample
returns solid black, appearing as dark patches at the top/bottom of the glass.
Clamping the UV to `[0.001, 0.999]` before the `Scene Color` sample forces
those edge pixels to read the nearest valid screen pixel instead of sampling
outside the texture, removing the black patches without changing the
magnification amount.

## Surface settings

| Setting | Value |
|---|---|
| Surface Type | Transparent |
| Render Face | Both (double-sided, so the glass reads correctly from inside and outside the tunnel) |
| Target | Universal Lit |

## Known platform limitation

`Scene Color` samples Unity's `_CameraOpaqueTexture`, which contains rendered
3D geometry only. It does not contain the AR camera's live video passthrough
in a WebGL/browser context, since the camera feed is composited outside
Unity's render pipeline there. In practice this means the magnification effect
is fully correct over rendered scene content (the fish, the tunnel itself) but
falls back to the shader's base tint over areas where only the live camera
feed sits behind the glass. This is a platform-level constraint of
WebGL AR rather than something fixable purely inside the shader graph.
