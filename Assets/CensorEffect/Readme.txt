== Usage ==
To add the effect to your scene:

- Go to GameObject -> 3D Object -> Quad
- Assign the "CensorEffectMat" material to the object

You can of course apply this to any kind of mesh.

== Shaders ==
All shaders can be found under the FX/ parent.

- Censor, applies the censor effect over the entire mesh
- Censor (Masked Cutout), takes an alpha map to mask out the effect (hard transitions)
- Censor (Masked Smooth), takes an alpha map to mask out the effect (smooth transitions)

== Known issues ==
Anti-aliasing may induce some flickering. Though if the censored object is slightly moving, this isn't noticable.