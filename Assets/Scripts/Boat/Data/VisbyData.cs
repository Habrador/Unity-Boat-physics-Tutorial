using UnityEngine;
using System.Collections;

public static class VisbyData
{
    //Speed o ship [m/s] = 35 knots
    public const float maxSpeed = 18f;
	//Length of ship [m]
	public const float length = 72.7f;
	//Width of ship (breath) [m]
	public const float width = 10.4f;

    //Engine - visby has a combined diesel or gas system (CODOG)
    //Diesel for low speeds [W]
    public const float total_power_low_speed = 2600000f;
	//Gas turbine for high speeds [W]
	public const float total_power_high_speed = 16000000f;

    // C_r - coefficient of air resistance (drag coefficient)
    //Between 0.6 and 1.1 for all boats, so have to estimate
    public const float C_r = 0.8f;

    //Total mass
    public const float mass = 600000f;
}
