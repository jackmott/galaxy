using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaxyShared.Networking.Messages;
using XnaGeometry;

namespace GalaxyShared.Networking
{
    public class Simulator
    {
        public static void UpdateState(GalaxyPlayer player, InputMessage input)
        {

            //   go.transform.Translate(Vector3.forward * input.Throttle * 40 * GalaxyClient.TICK_RATE);

            //rotate
            Matrix startRotation = Matrix.CreateFromQuaternion(player.rotation);
            Matrix changeRotation = Matrix.CreateFromYawPitchRoll(input.XTurn, input.YTurn, input.RollTurn);
            Matrix result = startRotation * changeRotation;
            player.rotation = Quaternion.CreateFromRotationMatrix(result);

            //translate

            player.PlayerPos += Vector3.Transform(Vector3.Forward * input.Throttle, result);
            player.Throttle = input.Throttle;


        }
    }
}
