using System.Runtime.InteropServices;
using AR.Drone.Data.Navigation.Native.Math;

namespace AR.Drone.Data.Navigation.Native.Options
{
	[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
	public struct navdata_magneto_t
	{
		public ushort tag;
		public ushort size;
		public short mx;
		public short my;
		public short mz;
		public vector31_t magneto_raw;
		public vector31_t magneto_rectified;
		public vector31_t magneto_offset;
		public float heading_unwrapped;
		public float heading_gyro_unwrapped;
		public float heading_fusion_unwrapped;
		public byte magneto_calibration_ok;
		public uint magneto_state;
		public float magneto_radius;
		public float error_mean;
		public float error_var;

		public override string ToString ()
		{
			string output = string.Format("{{tag: {0}}}\n", tag);
			output += string.Format("{{size: {0}}}\n", size);
			output += string.Format("{{mx: {0}}}\n", mx);
			output += string.Format("{{my: {0}}}\n", my);
			output += string.Format("{{mz: {0}}}\n", mz);
			output += string.Format("{{magneto_raw: X:{0} Y:{1} Z:{2}}}\n", magneto_raw.x, magneto_raw.y, magneto_raw.z);
			output += string.Format("{{magneto_rectified: X:{0} Y:{1} Z:{2}}}\n", magneto_rectified.x, magneto_rectified.y, magneto_rectified.z);;
			output += string.Format("{{magneto_offset: X:{0} Y:{1} Z:{2}}}\n", magneto_offset.x, magneto_offset.y, magneto_offset.z);
			output += string.Format("{{heading_unwrapped: {0}}}\n", heading_unwrapped);
			output += string.Format("{{heading_gyro_unwrapped: {0}}}\n", heading_gyro_unwrapped);
			output += string.Format("{{heading_fusion_unwrapped: {0}}}\n", heading_fusion_unwrapped);
			output += string.Format("{{magneto_calibration_ok: {0}}}\n", magneto_calibration_ok);
			output += string.Format("{{magneto_state: {0}}}\n", magneto_state);
			output += string.Format("{{magneto_radius: {0}}}\n", magneto_radius);
			output += string.Format("{{error_mean: {0}}}\n", error_mean);
			output += string.Format("{{error_var: {0}}}\n", error_var);

			return output;
		}
	}
}