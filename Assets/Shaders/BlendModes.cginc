/*
** Copyright (c) 2012, Romain Dura romain@shazbits.com
**
** Permission to use, copy, modify, and/or distribute this software for any
** purpose with or without fee is hereby granted, provided that the above
** copyright notice and this permission notice appear in all copies.
**
** THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
** WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
** MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY
** SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
** WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
** ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR
** IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
*/

/*
** Photoshop & misc math
** Blending modes
**
** Romain Dura | Romz
** Blog: http://mouaif.wordpress.com
** Post: http://mouaif.wordpress.com/?p=94
*/

/*
** Float blending modes
** Adapted from here: http://www.nathanm.com/photoshop-blending-math/
** But I modified the HardMix (wrong condition), Overlay, SoftLight, ColorDodge, ColorBurn, VividLight, PinLight (inverted layers) ones to have correct results
*/

#define BLEND_LINEAR_DODGE_F 			BLEND_ADD_F
#define BLEND_LINEAR_BURN_F 			BLEND_SUBSTRACT_F
#define BLEND_ADD_F(base, blend) 		min(base + blend, 1.0)
#define BLEND_SUBSTRACT_F(base, blend) 	max(base + blend - 1.0, 0.0)
#define BLEND_LIGHTEN_F(base, blend) 		max(blend, base)
#define BLEND_DARKEN_F(base, blend) 		min(blend, base)
#define BLEND_LINEAR_LIGHT_F(base, blend) 	(blend < 0.5 ? BLEND_LINEAR_BURN_F(base, (2.0 * blend)) : BLEND_LINEAR_DODGE_F(base, (2.0 * (blend - 0.5))))
#define BLEND_SCREEN_F(base, blend) 		(1.0 - ((1.0 - base) * (1.0 - blend)))
#define BLEND_OVERLAY_F(base, blend) 	(base < 0.5 ? (2.0 * base * blend) : (1.0 - 2.0 * (1.0 - base) * (1.0 - blend)))
#define BLEND_SOFT_LIGHT_F(base, blend) 	((blend < 0.5) ? (2.0 * base * blend + base * base * (1.0 - 2.0 * blend)) : (sqrt(base) * (2.0 * blend - 1.0) + 2.0 * base * (1.0 - blend)))
#define BLEND_COLOR_DODGE_F(base, blend) 	((blend == 1.0) ? blend : min(base / (1.0 - blend), 1.0))
#define BLEND_COLOR_BURN_F(base, blend) 	((blend == 0.0) ? blend : max((1.0 - ((1.0 - base) / blend)), 0.0))
#define BLEND_VIVID_LIGHT_F(base, blend) 	((blend < 0.5) ? BLEND_COLOR_BURN_F(base, (2.0 * blend)) : BLEND_COLOR_DODGE_F(base, (2.0 * (blend - 0.5))))
#define BLEND_PIN_LIGHT_F(base, blend) 	((blend < 0.5) ? BLEND_DARKEN_F(base, (2.0 * blend)) : BLEND_LIGHTEN_F(base, (2.0 *(blend - 0.5))))
#define BLEND_HARD_MIX_F(base, blend) 	((BLEND_VIVID_LIGHT_F(base, blend) < 0.5) ? 0.0 : 1.0)
#define BLEND_REFLECT_F(base, blend) 		((blend == 1.0) ? blend : min(base * base / (1.0 - blend), 1.0))


/*
** Vector3 blending modes
*/

// Component wise blending
#define BLEND(base, blend, funcf) 		fixed3(funcf(base.r, blend.r), funcf(base.g, blend.g), funcf(base.b, blend.b))

#define BLEND_NORMAL(base, blend) 		(blend)
#define BLEND_LIGHTEN				BLEND_LIGHTEN_F
#define BLEND_DARKEN				BLEND_DARKEN_F
#define BLEND_MULTIPLY(base, blend) 		(base * blend)
#define BLEND_AVERAGE(base, blend) 		((base + blend) / 2.0)
#define BLEND_ADD(base, blend) 		min(base + blend, fixed3(1.0))
#define BLEND_SUBTRACT(base, blend) 	max(base + blend - fixed3(1.0), fixed3(0.0))
#define BLEND_DIFFERENCE(base, blend) 	abs(base - blend)
#define BLEND_NEGATION(base, blend) 	(fixed3(1.0) - abs(fixed3(1.0) - base - blend))
#define BLEND_EXCLUSION(base, blend) 	(base + blend - 2.0 * base * blend)
#define BLEND_SCREEN(base, blend) 		BLEND(base, blend, BLEND_SCREEN_F)
#define BLEND_OVERLAY(base, blend) 		BLEND(base, blend, BLEND_OVERLAY_F)
#define BLEND_SOFT_LIGHT(base, blend) 	BLEND(base, blend, BLEND_SOFT_LIGHT_F)
#define BLEND_HARD_LIGHT(base, blend) 	BLEND_OVERLAY(blend, base)
#define BLEND_COLOR_DODGE(base, blend) 	BLEND(base, blend, BLEND_COLOR_DODGE_F)
#define BLEND_COLOR_BURN(base, blend) 	BLEND(base, blend, BLEND_COLOR_BURN_F)
#define BLEND_LINEAR_DODGE			BLEND_ADD
#define BLEND_LINEAR_BURN			BLEND_SUBTRACT
// Linear Light is another contrast-increasing mode
// If the blend color is darker than midgray, Linear Light darkens the image by decreasing the brightness. If the blend color is lighter than midgray, the result is a brighter image due to increased brightness.
#define BLEND_LINEAR_LIGHT(base, blend) 	BLEND(base, blend, BLEND_LINEAR_LIGHT_F)
#define BLEND_VIVID_LIGHT(base, blend) 	BLEND(base, blend, BLEND_VIVID_LIGHT_F)
#define BLEND_PIN_LIGHT(base, blend) 		BLEND(base, blend, BLEND_PIN_LIGHT_F)
#define BLEND_HARD_MIX(base, blend) 		BLEND(base, blend, BLEND_HARD_MIX_F)
#define BLEND_REFLECT(base, blend) 		BLEND(base, blend, BLEND_REFLECT_F)
#define BLEND_GLOW(base, blend) 		BLEND_REFLECT(blend, base)
#define BLEND_PHOENIX(base, blend) 		(min(base, blend) - max(base, blend) + fixed3(1.0))
#define BLEND_OPACITY(base, blend, F, O) 	(F(base, blend) * O + blend * (1.0 - O))
