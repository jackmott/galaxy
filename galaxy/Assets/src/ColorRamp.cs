using UnityEngine;


public class ColorRamp
{
    public static int RAMP_SIZE = 256;
    public Color[] Gradient = new Color[RAMP_SIZE];
    private Color[] Colors;
    private float[] Ranges;

    public ColorRamp(Color[] colors, float[] ranges)
    {
        Colors = colors;
        Ranges = ranges;
        int colorIndex = 0;
       // Gradient[RAMP_SIZE - 1] = Color.red;
        for (int i = 0; i < colors.Length - 1; i++)
        {
            Color start = colors[i];  //start of gradient
            Color end = colors[i + 1]; //end of gradient
            int indexSpan = (int)(RAMP_SIZE * Ranges[i]); // number of indices in colors to fill with this interpolation;

            for (int j = 0; j < indexSpan; j++)
            {
               // if (i == 0)
           //     {
              //      float r = start.r;
               //     float g = start.g;
              //      float b = start.b;
               //     float a = start.a;
               //     Gradient[colorIndex] = new Color(r, g, b, a);
              //  }
              //  else
              //  {
                    float r = Mathf.Lerp(start.r, end.r, (float)j / (float)indexSpan);
                    float g = Mathf.Lerp(start.g, end.g, (float)j / (float)indexSpan);
                    float b = Mathf.Lerp(start.b, end.b, (float)j / (float)indexSpan);
                    float a = Mathf.Lerp(start.a, end.a, (float)j / (float)indexSpan);
                    Gradient[colorIndex] = new Color(r, g, b, a);
              //  }

                colorIndex++;
            }

        }

        while (colorIndex < RAMP_SIZE)
        {
            Gradient[colorIndex] = colors[colors.Length - 1];
            colorIndex++;
        }

    }

    public override string ToString()
    {
        string result = "";
        for (int i = 0; i < Colors.Length; i++)
        {
            result += Colors[i].r.ToString("0.000") + "|"
            + Colors[i].g.ToString("0.000") + "|"
            + Colors[i].b.ToString("0.000") + "|"
            + Colors[i].a.ToString("0.000");
            if (i < Colors.Length - 1)
            {

            }
            if (i < Ranges.Length)
            {
                result += "," + Ranges[i] + ",";
            }
        }

        return result;
    }


}