using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace Assets.Scripts.Helper
{
    public enum ForkLiftState
    {
       eMove = 0,
       eLoadingPrepareElevator = 1,
       eLoadingPreparePosition = 2,
       eLoadingPallet = 3,
       eLoadingElevatorPallet = 4,
       eLoadingRetirePosition = 5,
       eLoadingSavePosition = 6,
       eUnLoadingPrepareElevator = 7,
       eUnLoadingPreparePosition = 8,
       eUnLoadingPallet = 9,
       eUnLoadingElevatorPallet = 10,
       eUnLoadingRetirePosition = 11,
       eUnLoadingSavePosition = 12,
       eNothing = 13,
       eRotate = 14

    }
}
