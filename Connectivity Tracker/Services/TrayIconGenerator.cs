using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Connectivity_Tracker.Services
{
    public class TrayIconGenerator
    {
        private const int IconSize = 16;
        private const int FontSize = 8;

        /// <summary>
        /// Generates a 16x16 icon with the ping value rendered on it.
        /// </summary>
        /// <param name="pingMs">The ping value in milliseconds, or null if ping failed</param>
        /// <returns>An Icon object suitable for system tray display</returns>
        public static Icon GeneratePingIcon(int? pingMs)
        {
            using (var bitmap = new Bitmap(IconSize, IconSize))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                // Set high-quality rendering
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

                // Determine colors based on ping value
                Color backgroundColor;
                Color textColor = Color.White;
                string text;

                if (!pingMs.HasValue)
                {
                    // Ping failed
                    backgroundColor = Color.FromArgb(200, 0, 0); // Dark red
                    text = "X";
                }
                else if (pingMs.Value < 50)
                {
                    backgroundColor = Color.FromArgb(0, 150, 0); // Green
                    text = FormatPingValue(pingMs.Value);
                }
                else if (pingMs.Value < 100)
                {
                    backgroundColor = Color.FromArgb(100, 180, 0); // Yellow-green
                    text = FormatPingValue(pingMs.Value);
                }
                else if (pingMs.Value < 200)
                {
                    backgroundColor = Color.FromArgb(230, 180, 0); // Yellow
                    text = FormatPingValue(pingMs.Value);
                }
                else
                {
                    backgroundColor = Color.FromArgb(200, 0, 0); // Red
                    text = FormatPingValue(pingMs.Value);
                }

                // Fill background
                graphics.Clear(backgroundColor);

                // Draw border
                using (var borderPen = new Pen(Color.FromArgb(100, 0, 0, 0), 1))
                {
                    graphics.DrawRectangle(borderPen, 0, 0, IconSize - 1, IconSize - 1);
                }

                // Draw text
                using (var font = new Font("Arial", FontSize, FontStyle.Bold, GraphicsUnit.Pixel))
                {
                    var textSize = graphics.MeasureString(text, font);
                    var x = (IconSize - textSize.Width) / 2;
                    var y = (IconSize - textSize.Height) / 2;

                    // Draw shadow for better readability
                    using (var shadowBrush = new SolidBrush(Color.FromArgb(100, 0, 0, 0)))
                    {
                        graphics.DrawString(text, font, shadowBrush, x + 1, y + 1);
                    }

                    // Draw main text
                    using (var textBrush = new SolidBrush(textColor))
                    {
                        graphics.DrawString(text, font, textBrush, x, y);
                    }
                }

                // Convert bitmap to icon
                IntPtr hIcon = bitmap.GetHicon();
                Icon icon = Icon.FromHandle(hIcon);
                return (Icon)icon.Clone();
            }
        }

        /// <summary>
        /// Formats the ping value to fit in the small icon space.
        /// Values >= 1000ms are shown as "1K", etc.
        /// </summary>
        private static string FormatPingValue(int pingMs)
        {
            if (pingMs >= 1000)
            {
                return $"{pingMs / 1000}K";
            }
            else if (pingMs >= 100)
            {
                // Show only 2 digits for values >= 100
                return pingMs.ToString();
            }
            else
            {
                return pingMs.ToString();
            }
        }

        /// <summary>
        /// Generates a 16x16 icon with the packet loss percentage rendered on it.
        /// </summary>
        /// <param name="packetLossPercentage">The packet loss percentage (0-100)</param>
        /// <returns>An Icon object suitable for system tray display</returns>
        public static Icon GeneratePacketLossIcon(double packetLossPercentage)
        {
            using (var bitmap = new Bitmap(IconSize, IconSize))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                // Set high-quality rendering
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

                // Determine colors based on packet loss percentage
                Color backgroundColor;
                Color textColor = Color.White;
                string text = FormatPacketLossValue(packetLossPercentage);

                if (packetLossPercentage < 1)
                {
                    backgroundColor = Color.FromArgb(0, 150, 0); // Green - excellent
                }
                else if (packetLossPercentage < 5)
                {
                    backgroundColor = Color.FromArgb(100, 180, 0); // Yellow-green - acceptable
                }
                else if (packetLossPercentage < 10)
                {
                    backgroundColor = Color.FromArgb(230, 180, 0); // Yellow - concerning
                }
                else
                {
                    backgroundColor = Color.FromArgb(200, 0, 0); // Red - critical
                }

                // Fill background
                graphics.Clear(backgroundColor);

                // Draw border
                using (var borderPen = new Pen(Color.FromArgb(100, 0, 0, 0), 1))
                {
                    graphics.DrawRectangle(borderPen, 0, 0, IconSize - 1, IconSize - 1);
                }

                // Draw text
                using (var font = new Font("Arial", FontSize, FontStyle.Bold, GraphicsUnit.Pixel))
                {
                    var textSize = graphics.MeasureString(text, font);
                    var x = (IconSize - textSize.Width) / 2;
                    var y = (IconSize - textSize.Height) / 2;

                    // Draw shadow for better readability
                    using (var shadowBrush = new SolidBrush(Color.FromArgb(100, 0, 0, 0)))
                    {
                        graphics.DrawString(text, font, shadowBrush, x + 1, y + 1);
                    }

                    // Draw main text
                    using (var textBrush = new SolidBrush(textColor))
                    {
                        graphics.DrawString(text, font, textBrush, x, y);
                    }
                }

                // Convert bitmap to icon
                IntPtr hIcon = bitmap.GetHicon();
                Icon icon = Icon.FromHandle(hIcon);
                return (Icon)icon.Clone();
            }
        }

        /// <summary>
        /// Formats the packet loss percentage to fit in the small icon space.
        /// </summary>
        private static string FormatPacketLossValue(double packetLossPercentage)
        {
            if (packetLossPercentage >= 100)
            {
                return "99+";
            }
            else if (packetLossPercentage >= 10)
            {
                // Show only whole number for double digits (e.g., "15")
                return ((int)packetLossPercentage).ToString();
            }
            else if (packetLossPercentage >= 1)
            {
                // Show one decimal place for single digits (e.g., "5.2")
                return packetLossPercentage.ToString("F1");
            }
            else if (packetLossPercentage > 0)
            {
                // Show "0" for very small values
                return "0";
            }
            else
            {
                // Perfect - no packet loss
                return "0";
            }
        }
    }
}
