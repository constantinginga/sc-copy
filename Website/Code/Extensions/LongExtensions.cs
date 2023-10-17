namespace StartupCentral.Extensions
{
    public static class LongExtensions
    {
        public static string FormatBytes(this long num_bytes)
        {
            const double ONE_KB = 1024;
            const double ONE_MB = ONE_KB * 1024;
            const double ONE_GB = ONE_MB * 1024;
            const double ONE_TB = ONE_GB * 1024;
            const double ONE_PB = ONE_TB * 1024;
            const double ONE_EB = ONE_PB * 1024;
            const double ONE_ZB = ONE_EB * 1024;
            const double ONE_YB = ONE_ZB * 1024;

            // See how big the value is.
            if (num_bytes <= 999)
            {
                // Format in bytes.
                return string.Format(num_bytes.ToString(), "0") + " bytes";
            }
            else if (num_bytes <= ONE_KB * 999)
            {
                // Format in KB.
                return ThreeNonZeroDigits(num_bytes / ONE_KB) + " " + "KB";
            }
            else if (num_bytes <= ONE_MB * 999)
            {
                // Format in MB.
                return ThreeNonZeroDigits(num_bytes / ONE_MB) + " " + "MB";
            }
            else if (num_bytes <= ONE_GB * 999)
            {
                // Format in GB.
                return ThreeNonZeroDigits(num_bytes / ONE_GB) + " " + "GB";
            }
            else if (num_bytes <= ONE_TB * 999)
            {
                // Format in TB.
                return ThreeNonZeroDigits(num_bytes / ONE_TB) + " " + "TB";
            }
            else if (num_bytes <= ONE_PB * 999)
            {
                // Format in PB.
                return ThreeNonZeroDigits(num_bytes / ONE_PB) + " " + "PB";
            }
            else if (num_bytes <= ONE_EB * 999)
            {
                // Format in EB.
                return ThreeNonZeroDigits(num_bytes / ONE_EB) + " " + "EB";
            }
            else if (num_bytes <= ONE_ZB * 999)
            {
                // Format in ZB.
                return ThreeNonZeroDigits(num_bytes / ONE_ZB) + " " + "ZB";
            }
            else
            {
                // Format in YB.
                return ThreeNonZeroDigits(num_bytes / ONE_YB) + " " + "YB";
            }
        }

        // Return the value formatted to include at most three
        // non-zero digits and at most two digits after the
        // decimal point. Examples:
        //         1
        //       123
        //        12.3
        //         1.23
        //         0.12
        private static string ThreeNonZeroDigits(double value)
        {
            if (value >= 100)
            {
                // No digits after the decimal.
                return value.ToString("0");
            }
            else if (value >= 10)
            {
                // One digit after the decimal.
                return value.ToString("0.0");
            }
            else
            {
                // Two digits after the decimal.
                return value.ToString("0.00");
            }
        }

        /// <summary>
        /// Extension that quick and easy can tell you if the specific integer is between two values
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="MinValue">Lower value</param>
        /// <param name="MaxValue">Upper value</param>
        /// <returns></returns>
        /// <remarks></remarks>

        public static bool Between(this long Value, long MinValue, long MaxValue)
        {
            return (Value >= MinValue & Value <= MaxValue);
        }
    }
}