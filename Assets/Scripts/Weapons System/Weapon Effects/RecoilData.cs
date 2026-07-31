using System;

[Serializable]
public struct RecoilData
{
    public float RecoilX;
    public float RecoilY;
    public float RecoilZ;
    public float KickbackZ;

    public RecoilData(float RecoilX, float RecoilY, float RecoilZ, float KickbackZ)
    {
        this.RecoilX = RecoilX;
        this.RecoilY = RecoilY;
        this.RecoilZ = RecoilZ;
        this.KickbackZ = KickbackZ;
    }
}
