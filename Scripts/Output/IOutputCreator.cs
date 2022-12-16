using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaveFunctionCollapse
{
    public interface IOutputCreator<T>
    {
        T OutputImage { get; }
        void CreateOutput(PatternManager manager, int[][] outputvalues, int width, int height);
    }

}
