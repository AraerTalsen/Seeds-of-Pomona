using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface IRuntimeLauncher
{
    public Task<IRuntimeEvent> LaunchEffect(EffectContext context);
}
