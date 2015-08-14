using UnityEngine;

public static class ColorTemperature
{
    //Start with a temperature, in Kelvin, somewhere between 1000 and 40000.  (Other values may work,
    //but I can't make any promises about the quality of the algorithm's estimates above 40000 K.)
    //Note also that the temperature and color variables need to be declared as floating-point.
    public static Color Color(float temperature)
    {
        temperature /= 100;

        Color color = new Color();

        //Calculate Red:
        if (temperature <= 66)
            color.r = 1.0f;
        else
            color.r = 1.292936186062745f * Mathf.Pow(temperature - 60, -0.1332047592f);

        color.r = Mathf.Clamp(color.r, 0, 1);

        //Calculate Green:
        if (temperature <= 66)
            color.g = 0.3900815787690196f * Mathf.Log(temperature) - 0.6318414437886275f;
        else
            color.g = 1.129890860895294f * Mathf.Pow(temperature - 60, -0.0755148492f);

        color.g = Mathf.Clamp(color.g, 0, 1);

        //Calculate Blue:
        if (temperature >= 66)
            color.b = 1.0f;
        else if (temperature <= 19)
            color.b = 0;
        else
            color.b = 0.5432067891101961f * Mathf.Log(temperature - 10) - 1.19625408914f;
        color.b = Mathf.Clamp(color.b, 0, 1);

        color.a = 1;

        return color;
    }
}
