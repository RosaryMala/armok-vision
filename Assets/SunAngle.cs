using UnityEngine;
using System;

public static class SunAngle
{
    private const double Deg2Rad = Math.PI / 180.0;
    private const double Rad2Deg = 180.0 / Math.PI;
    private const int TicksPerDay = 1200;
    private const int DaysPerYear = 336;

    /*!
     * \brief Calculates the sun light.
     *
     * CalcSunPosition calculates the suns "position" based on a
     * given date and time in local time, latitude and longitude
     * expressed in decimal degrees. It is based on the method
     * found here:
     * http://www.astro.uio.no/~bgranslo/aares/calculate.html
     * The calculation is only satisfiably correct for dates in
     * the range March 1 1900 to February 28 2100.
     * \param dateTime Time and date in local time.
     * \param latitude Latitude expressed in decimal degrees.
     * \param longitude Longitude expressed in decimal degrees.
     */
    public static Vector3 CalculateSunPosition(
        int ticks, int year, double latitude)
    {
        //// Convert to UTC
        //dateTime = dateTime.ToUniversalTime();

        // Number of days from J2000.0.
        double julianDate = (double)year * (double)DaysPerYear + (double)ticks / (double)TicksPerDay;

        double julianCenturies = julianDate / (DaysPerYear * 100);

        //// Sidereal Time
        //double siderealTimeTicks = DaysPerYear * TicksPerDay * 100 * julianCenturies;

        //double siderealTimeUT = siderealTimeHours +
        //    (366.2422 / 365.2422) * (double)dateTime.TimeOfDay.TotalHours;

        double siderealTime = (ticks % TicksPerDay) * 360 / (double)TicksPerDay;

        //// Refine to number of days (fractional) to specific time.
        //julianDate += (double)dateTime.TimeOfDay.TotalHours / 24.0;
        //julianCenturies = julianDate / 36525.0;

        // Solar Coordinates
        double meanLongitude = CorrectAngle(Deg2Rad *
            (280.466 + 36000.77 * julianCenturies));

        double meanAnomaly = CorrectAngle(Deg2Rad *
            (357.529 + 35999.05 * julianCenturies));

        double equationOfCenter = Deg2Rad * ((1.915 - 0.005 * julianCenturies) *
            Math.Sin(meanAnomaly) + 0.02 * Math.Sin(2 * meanAnomaly));

        double elipticalLongitude =
            CorrectAngle(meanLongitude + equationOfCenter);

        double obliquity = (23.439 - 0.013 * julianCenturies) * Deg2Rad;

        // Right Ascension
        double rightAscension = Math.Atan2(
            Math.Cos(obliquity) * Math.Sin(elipticalLongitude),
            Math.Cos(elipticalLongitude));

        double declination = Math.Asin(
            Math.Sin(rightAscension) * Math.Sin(obliquity));

        // Horizontal Coordinates
        double hourAngle = CorrectAngle(siderealTime * Deg2Rad) - rightAscension;

        if (hourAngle > Math.PI)
        {
            hourAngle -= 2 * Math.PI;
        }

        double altitude = Math.Asin(Math.Sin(latitude * Deg2Rad) *
            Math.Sin(declination) + Math.Cos(latitude * Deg2Rad) *
            Math.Cos(declination) * Math.Cos(hourAngle));

        // Nominator and denominator for calculating Azimuth
        // angle. Needed to test which quadrant the angle is in.
        double aziNom = -Math.Sin(hourAngle);
        double aziDenom =
            Math.Tan(declination) * Math.Cos(latitude * Deg2Rad) -
            Math.Sin(latitude * Deg2Rad) * Math.Cos(hourAngle);

        double azimuth = Math.Atan(aziNom / aziDenom);

        if (aziDenom < 0) // In 2nd or 3rd quadrant
        {
            azimuth += Math.PI;
        }
        else if (aziNom < 0) // In 4th quadrant
        {
            azimuth += 2 * Math.PI;
        }

        return new Vector3((float)(altitude * Rad2Deg), 180.0f + (float)(azimuth * Rad2Deg), 0);
    }



    /*!
    * \brief Corrects an angle.
    *
    * \param angleInRadians An angle expressed in radians.
    * \return An angle in the range 0 to 2*PI.
    */
    private static double CorrectAngle(double angleInRadians)
    {
        if (angleInRadians < 0)
        {
            return 2 * Math.PI - (Math.Abs(angleInRadians) % (2 * Math.PI));
        }
        else if (angleInRadians > 2 * Math.PI)
        {
            return angleInRadians % (2 * Math.PI);
        }
        else
        {
            return angleInRadians;
        }
    }
}
