#ifndef CUSTOM_METALLIC_INCLUDED
#define CUSTOM_METALLIC_INCLUDED

        half3 DiffuseAndSpecularFromMetallicCustom (half3 albedo, half metallic, out half3 specColor, out half oneMinusReflectivity)
        {
            float oneMinusDielectricSpec = 1 - (_SpecColor.r * 0.3 + _SpecColor.g * 0.59 + _SpecColor.b * 0.11);

            specColor = lerp (_SpecColor.rgb, albedo, metallic);
            oneMinusReflectivity = oneMinusDielectricSpec - metallic * oneMinusDielectricSpec;
            return albedo * oneMinusReflectivity;
        }
#endif