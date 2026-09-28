using System;
using System.Collections.Generic;
using UnityEngine;

namespace NutriAR.Services
{
    /// <summary>Small, dependency-free EAN-13 reader for clear, front-facing camera frames.</summary>
    public static class EAN13Decoder
    {
        private static readonly string[] LeftOdd =
        {
            "0001101", "0011001", "0010011", "0111101", "0100011",
            "0110001", "0101111", "0111011", "0110111", "0001011"
        };

        private static readonly string[] LeftEven =
        {
            "0100111", "0110011", "0011011", "0100001", "0011101",
            "0111001", "0000101", "0010001", "0001001", "0010111"
        };

        private static readonly string[] Right =
        {
            "1110010", "1100110", "1101100", "1000010", "1011100",
            "1001110", "1010000", "1000100", "1001000", "1110100"
        };

        private static readonly string[] FirstDigitParity =
        {
            "LLLLLL", "LLGLGG", "LLGGLG", "LLGGGL", "LGLLGG",
            "LGGLLG", "LGGGLL", "LGLGLG", "LGLGGL", "LGGLGL"
        };

        public static bool TryDecode(Color32[] pixels, int width, int height, out string barcode)
        {
            barcode = null;
            if (pixels == null || width < 200 || height < 80 || pixels.Length < width * height)
            {
                return false;
            }

            var buffer = new List<byte>(Mathf.Max(width, height));
            var transitions = new List<int>(Mathf.Max(width, height) / 2);
            var horizontalSteps = Mathf.Clamp(height / 28, 1, 20);
            for (var i = 0; i <= horizontalSteps; i++)
            {
                var y = Mathf.Clamp(Mathf.RoundToInt((i + 0.5f) * height / (horizontalSteps + 1f)), 0, height - 1);
                if (DecodeLine(pixels, width, height, y, true, false, buffer, transitions, out barcode) ||
                    DecodeLine(pixels, width, height, y, true, true, buffer, transitions, out barcode))
                {
                    return true;
                }
            }

            var verticalSteps = Mathf.Clamp(width / 28, 1, 20);
            for (var i = 0; i <= verticalSteps; i++)
            {
                var x = Mathf.Clamp(Mathf.RoundToInt((i + 0.5f) * width / (verticalSteps + 1f)), 0, width - 1);
                if (DecodeLine(pixels, width, height, x, false, false, buffer, transitions, out barcode) ||
                    DecodeLine(pixels, width, height, x, false, true, buffer, transitions, out barcode))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool DecodeLine(Color32[] pixels, int width, int height, int fixedCoordinate, bool horizontal, bool reverse, List<byte> bits, List<int> transitions, out string barcode)
        {
            barcode = null;
            var length = horizontal ? width : height;
            var min = 255;
            var max = 0;
            for (var i = 0; i < length; i += 2)
            {
                var pixel = GetPixel(pixels, width, height, fixedCoordinate, i, horizontal);
                var gray = (pixel.r * 299 + pixel.g * 587 + pixel.b * 114) / 1000;
                if (gray < min) min = gray;
                if (gray > max) max = gray;
            }

            if (max - min < 45)
            {
                return false;
            }

            var threshold = (min + max) / 2;
            bits.Clear();
            transitions.Clear();
            byte previous = IsDark(GetPixel(pixels, width, height, fixedCoordinate, reverse ? length - 1 : 0, horizontal), threshold) ? (byte)1 : (byte)0;
            bits.Add(previous);
            transitions.Add(0);
            for (var i = 1; i < length; i++)
            {
                var sourceIndex = reverse ? length - 1 - i : i;
                var dark = IsDark(GetPixel(pixels, width, height, fixedCoordinate, sourceIndex, horizontal), threshold) ? (byte)1 : (byte)0;
                if (dark != previous)
                {
                    bits.Add(dark);
                    transitions.Add(i);
                    previous = dark;
                }
            }

            var runCount = bits.Count;
            for (var run = 0; run + 2 < runCount; run++)
            {
                if (bits[run] != 1 || bits[run + 1] != 0 || bits[run + 2] != 1)
                {
                    continue;
                }

                var start = transitions[run];

                var firstWidth = MeasureRun(pixels, width, height, fixedCoordinate, horizontal, reverse, threshold, start, length, true);
                var secondStart = start + firstWidth;
                var secondWidth = MeasureRun(pixels, width, height, fixedCoordinate, horizontal, reverse, threshold, secondStart, length, false);
                var thirdStart = secondStart + secondWidth;
                var thirdWidth = MeasureRun(pixels, width, height, fixedCoordinate, horizontal, reverse, threshold, thirdStart, length, true);
                if (firstWidth <= 0 || secondWidth <= 0 || thirdWidth <= 0)
                {
                    continue;
                }

                var moduleWidth = (firstWidth + secondWidth + thirdWidth) / 3f;
                if (moduleWidth < 1.15f ||
                    firstWidth > moduleWidth * 1.85f || secondWidth > moduleWidth * 1.85f || thirdWidth > moduleWidth * 1.85f ||
                    firstWidth < moduleWidth * 0.45f || secondWidth < moduleWidth * 0.45f || thirdWidth < moduleWidth * 0.45f)
                {
                    continue;
                }

                var sampleStart = bits.Count;
                for (var module = 0; module < 95; module++)
                {
                    var sample = start + Mathf.FloorToInt((module + 0.5f) * moduleWidth);
                    if (sample < 0 || sample >= length)
                    {
                        break;
                    }
                    var sourceIndex = reverse ? length - 1 - sample : sample;
                    bits.Add(IsDark(GetPixel(pixels, width, height, fixedCoordinate, sourceIndex, horizontal), threshold) ? (byte)1 : (byte)0);
                }

                if (bits.Count - sampleStart == 95 && TryDecodeModules(bits, sampleStart, out barcode))
                {
                    return true;
                }

                bits.RemoveRange(sampleStart, bits.Count - sampleStart);
            }

            return false;
        }

        private static int MeasureRun(Color32[] pixels, int width, int height, int fixedCoordinate, bool horizontal, bool reverse, int threshold, int start, int length, bool expectedDark)
        {
            if (start < 0 || start >= length)
            {
                return 0;
            }

            var count = 0;
            for (var i = start; i < length; i++)
            {
                var sourceIndex = reverse ? length - 1 - i : i;
                var dark = IsDark(GetPixel(pixels, width, height, fixedCoordinate, sourceIndex, horizontal), threshold);
                if (dark != expectedDark)
                {
                    break;
                }
                count++;
            }
            return count;
        }

        private static bool TryDecodeModules(List<byte> bits, int offset, out string barcode)
        {
            barcode = null;
            if (offset < 0 || offset + 95 > bits.Count || !Match(bits, offset, "101") || !Match(bits, offset + 45, "01010") || !Match(bits, offset + 92, "101"))
            {
                return false;
            }

            var leftDigits = new int[6];
            var parity = "";
            for (var digit = 0; digit < 6; digit++)
            {
                var pattern = ReadBits(bits, offset + 3 + digit * 7, 7);
                var value = Array.IndexOf(LeftOdd, pattern);
                if (value >= 0)
                {
                    leftDigits[digit] = value;
                    parity += "L";
                    continue;
                }

                value = Array.IndexOf(LeftEven, pattern);
                if (value < 0)
                {
                    return false;
                }
                leftDigits[digit] = value;
                parity += "G";
            }

            var firstDigit = Array.IndexOf(FirstDigitParity, parity);
            if (firstDigit < 0)
            {
                return false;
            }

            var digits = new char[13];
            digits[0] = (char)('0' + firstDigit);
            for (var i = 0; i < 6; i++)
            {
                digits[i + 1] = (char)('0' + leftDigits[i]);
                var right = Array.IndexOf(Right, ReadBits(bits, offset + 50 + i * 7, 7));
                if (right < 0)
                {
                    return false;
                }
                digits[i + 7] = (char)('0' + right);
            }

            var text = new string(digits);
            if (!HasValidChecksum(text))
            {
                return false;
            }
            barcode = text;
            return true;
        }

        public static bool HasValidChecksum(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length != 13)
            {
                return false;
            }

            var sum = 0;
            for (var i = 0; i < 12; i++)
            {
                if (value[i] < '0' || value[i] > '9') return false;
                sum += (value[i] - '0') * (i % 2 == 0 ? 1 : 3);
            }
            return value[12] - '0' == (10 - sum % 10) % 10;
        }

        private static bool Match(List<byte> bits, int offset, string expected)
        {
            for (var i = 0; i < expected.Length; i++)
            {
                if (bits[offset + i] != (expected[i] == '1' ? 1 : 0)) return false;
            }
            return true;
        }

        private static string ReadBits(List<byte> bits, int offset, int count)
        {
            var chars = new char[count];
            for (var i = 0; i < count; i++) chars[i] = bits[offset + i] == 1 ? '1' : '0';
            return new string(chars);
        }

        private static bool IsDark(Color32 pixel, int threshold)
        {
            return (pixel.r * 299 + pixel.g * 587 + pixel.b * 114) / 1000 < threshold;
        }

        private static Color32 GetPixel(Color32[] pixels, int width, int height, int fixedCoordinate, int position, bool horizontal)
        {
            return horizontal ? pixels[fixedCoordinate * width + position] : pixels[position * width + fixedCoordinate];
        }
    }
}
