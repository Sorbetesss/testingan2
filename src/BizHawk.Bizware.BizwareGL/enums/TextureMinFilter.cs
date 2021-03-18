namespace BizHawk.Bizware.BizwareGL
{
	public enum TextureMinFilter
	{
		Nearest = 0x2600,
		Linear = 0x2601,
		NearestMipmapNearest = 0x2700,
		LinearMipmapNearest = 0x2701,
		NearestMipmapLinear = 0x2702,
		LinearMipmapLinear = 0x2703,
		Filter4Sgis = 0x8146,
		LinearClipmapLinearSgix = 0x8170,
		PixelTexGenQCeilingSgix = 0x8184,
		PixelTexGenQRoundSgix = 0x8185,
		PixelTexGenQFloorSgix = 0x8186,
		NearestClipmapNearestSgix = 0x844D,
		NearestClipmapLinearSgix = 0x844E,
		LinearClipmapNearestSgix = 0x844F,
	}
}
