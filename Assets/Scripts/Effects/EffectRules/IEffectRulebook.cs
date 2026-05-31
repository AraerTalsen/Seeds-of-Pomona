using System;
using System.Collections;
using System.Collections.Generic;

public interface IEffectRulebook<T> where T : Affecter<T>
{
    protected Dictionary<int, Action<EffectContext, T>> EffectDirectory { get; }
    protected bool[] EffectConfig(T payload);

    public Action<EffectContext, T> GetCurrentEffect(T payload) => EffectDirectory[ConfigToID(EffectConfig(payload))];

    private static int ConfigToID(bool[] config)
    {
        BitArray boolToBits = new (config);
        byte[] bitToByte = new byte[1];
        boolToBits.CopyTo(bitToByte, 0);

        return bitToByte[0];
    }
}
