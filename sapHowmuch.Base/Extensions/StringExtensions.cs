using sapHowmuch.Base.Enums;
using sapHowmuch.Base.Helpers;
using System;
using System.Globalization;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace sapHowmuch.Base.Extensions
{
	public static class StringExtensions
	{
		/// <summary>
		/// Truncate string
		/// </summary>
		/// <param name="value">String to truncate</param>
		/// <param name="maxLength">Max Length</param>
		/// <returns>Truncated string</returns>
		public static string Truncate(this string value, int maxLength)
		{
			if (string.IsNullOrEmpty(value)) return value;

			return value.Length <= maxLength ? value : value.Substring(0, maxLength);
		}

		/// <summary>
		/// Truncate string and warn log if truncaded
		/// </summary>
		/// <param name="value">String to truncate</param>
		/// <param name="maxLength">Max Length</param>
		/// <returns>Truncated string</returns>
		public static string TruncateAndWarn(this string value, int maxLength)
		{
			if (string.IsNullOrEmpty(value)) return value;

			var truncated = value.Length <= maxLength ? value : value.Substring(0, maxLength);

			sapHowmuchLogger.Warn($"Truncating '{value}' to '{truncated}' (Max {maxLength})");

			return truncated;
		}

		public static byte[] ToByteArray(this string str)
		{
			byte[] strBytes = Encoding.UTF8.GetBytes(str);
			return strBytes;
		}

		/// <summary>
		/// 해쉬 문자열 반환
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static string GetHash(this string input)
		{
			HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();
			byte[] byteValue = input.ToByteArray();
			byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);

			return Convert.ToBase64String(byteHash);
		}

		public static string GetHash(this string input, HashType hashType)
		{
			byte[] inputBytes = input.ToByteArray();

			switch (hashType)
			{
				case HashType.HMAC: return Convert.ToBase64String(HMAC.Create().ComputeHash(inputBytes));
				case HashType.HMACMD5: return Convert.ToBase64String(HMAC.Create().ComputeHash(inputBytes));
				case HashType.HMACSHA1: return Convert.ToBase64String(HMAC.Create().ComputeHash(inputBytes));
				case HashType.HMACSHA256: return Convert.ToBase64String(HMAC.Create().ComputeHash(inputBytes));
				case HashType.HMACSHA384: return Convert.ToBase64String(HMAC.Create().ComputeHash(inputBytes));
				case HashType.HMACSHA512: return Convert.ToBase64String(HMAC.Create().ComputeHash(inputBytes));
				case HashType.MACTripleDES: return Convert.ToBase64String(KeyedHashAlgorithm.Create().ComputeHash(inputBytes));
				case HashType.MD5: return Convert.ToBase64String(MD5.Create().ComputeHash(inputBytes));
				case HashType.RIPEMD160: return Convert.ToBase64String(RIPEMD160.Create().ComputeHash(inputBytes));
				case HashType.SHA1: return Convert.ToBase64String(SHA1.Create().ComputeHash(inputBytes));
				case HashType.SHA256: return Convert.ToBase64String(SHA256.Create().ComputeHash(inputBytes));
				case HashType.SHA384: return Convert.ToBase64String(SHA384.Create().ComputeHash(inputBytes));
				case HashType.SHA512: return Convert.ToBase64String(SHA512.Create().ComputeHash(inputBytes));
				default: return Convert.ToBase64String(inputBytes);
			}
		}

		/// <summary>
		/// key 를 이용해 문자열 암호화, RSA 이용
		/// </summary>
		/// <param name="stringToEncrypt">암호화할 문자열</param>
		/// <param name="key">암호키</param>
		/// <returns>마이너스 부호로 구분된 byte array 형 문자열 반환</returns>
		/// <exception cref="ArgumentException">암호화할 문자열이나 암호키가 null 이거나 빈문자열 일때 발생</exception>
		public static string Encrypt(this string stringToEncrypt, string key)
		{
			if (string.IsNullOrEmpty(stringToEncrypt)) throw new ArgumentException("빈 문자열은 암호화할 수 없습니다.");

			if (string.IsNullOrEmpty(key)) throw new ArgumentException("암호키가 빈문자열 입니다. 암호키를 제공하십시오.");

			CspParameters cspp = new CspParameters();
			cspp.KeyContainerName = key;

			RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cspp);
			rsa.PersistKeyInCsp = true;

			byte[] bytes = rsa.Encrypt(System.Text.UTF8Encoding.UTF8.GetBytes(stringToEncrypt), true);

			return BitConverter.ToString(bytes);
		}

		/// <summary>
		/// key 를 이용해 문자열 복호화, RSA 알고리즘
		/// </summary>
		/// <param name="stringToDecrypt"></param>
		/// <param name="key"></param>
		/// <returns>복호화된 문자열 혹은 복호화 실패 시 null 리턴</returns>
		public static string Decrypt(this string stringToDecrypt, string key)
		{
			string result = null;

			if (string.IsNullOrEmpty(stringToDecrypt)) throw new ArgumentException("빈 문자열은 복호화할 수 없습니다.");

			if (string.IsNullOrEmpty(key)) throw new ArgumentException("암호키가 빈문자열 입니다. 암호키를 제공하십시오.");

			try
			{
				CspParameters cspp = new CspParameters();
				cspp.KeyContainerName = key;

				RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cspp);
				rsa.PersistKeyInCsp = true;

				string[] decryptArray = stringToDecrypt.Split(new string[] { "-" }, StringSplitOptions.None);
				byte[] decryptByteArray = Array.ConvertAll<string, byte>(decryptArray, (s => Convert.ToByte(byte.Parse(s, System.Globalization.NumberStyles.HexNumber))));

				byte[] bytes = rsa.Decrypt(decryptByteArray, true);

				result = System.Text.UTF8Encoding.UTF8.GetString(bytes);
			}
			finally
			{
			}

			return result;
		}

		/// <summary>
		/// stringValues 에 value 가 포함되어 있는지 여부
		/// </summary>
		/// <param name="value"></param>
		/// <param name="stringValues"></param>
		/// <returns></returns>
		public static bool In(this string value, params string[] stringValues)
		{
			foreach (string otherValue in stringValues)
			{
				if (string.Compare(value, otherValue) == 0) return true;
			}

			return false;
		}

		/// <summary>
		/// chars 에 value 가 포함되어 잇는지 여부
		/// </summary>
		/// <param name="value"></param>
		/// <param name="chars"></param>
		/// <returns></returns>
		public static bool In(this char value, params char[] chars)
		{
			foreach (char c in chars)
			{
				if (c == value) return true;
			}

			return false;
		}

		/// <summary>
		/// 문자열을 Enum 으로 변환
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static T ToEnum<T>(this string value) where T : struct
		{
			return (T)Enum.Parse(typeof(T), value, true);
		}

		/// <summary>
		/// string.Format 확장
		/// </summary>
		/// <param name="value"></param>
		/// <param name="arg0"></param>
		/// <returns></returns>
		public static string Format(this string value, object arg0)
		{
			return string.Format(value, arg0);
		}

		/// <summary>
		/// string.Format 확장 오버로딩
		/// </summary>
		/// <param name="value"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public static string Format(this string value, params object[] args)
		{
			return string.Format(value, args);
		}

		/// <summary>
		/// 마스킹 적용
		/// <para>
		/// </para>
		/// <example>
		/// var s = "aaaaaaaabbbbccccddddeeeeeeeeeeee".FormatWithMask("Hello ########-#A###-####-####-############ Oww");
		///		s.ShouldEqual("Hello aaaaaaaa-bAbbb-cccc-dddd-eeeeeeeeeeee Oww");
		///
		/// var s = "abc".FormatWithMask("###-#");
		///		s.ShouldEqual("abc-");
		/// var s = "".FormatWithMask("Hello ########-#A###-####-####-############ Oww");
		///		s.ShouldEqual("");
		/// </example>
		/// </summary>
		/// <param name="input"></param>
		/// <param name="mask"></param>
		/// <returns></returns>
		public static string FormatWithMask(this string input, string mask)
		{
			if (string.IsNullOrEmpty(input)) return input;

			var output = string.Empty;
			var index = 0;

			foreach (var m in mask)
			{
				if (m == '#')
				{
					if (index < input.Length)
					{
						output += input[index];
						index++;
					}
				}
				else
				{
					output += m;
				}
			}
			return output;
		}

		/// <summary>
		/// Hex 로 표현된 문자열을 byte[] 배열로 변환
		/// </summary>
		/// <param name="hexString"></param>
		/// <returns></returns>
		public static byte[] HexToBytes(this string hexString)
		{
			if (hexString.Length % 2 != 0)
				throw new ArgumentException(string.Format("HexString cannot be in odd number: {0}", hexString));

			var retVal = new byte[hexString.Length / 2];

			for (int i = 0; i < hexString.Length; i += 2)
			{
				retVal[i / 2] = byte.Parse(hexString.Substring(i, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
			}

			return retVal;
		}

		/// <summary>
		/// Tag 벗기기
		/// 좀 더 개선 필요, 특정 태그만을 받는다덜지, XML 관련도 테스트 필요
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static string StripHTMLTag(this string input)
		{
			var tagsExpression = new Regex(@"</?.+?>");

			return tagsExpression.Replace(input, " ");
		}

		public static T Deserialize<T>(this XDocument xmlDoc)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			using (XmlReader reader = xmlDoc.CreateReader())
			{
				return (T)xmlSerializer.Deserialize(reader);
			}
		}

		public static SecureString ToSecureString(this string str)
		{
			SecureString secureString = new SecureString();

			foreach (char c in str)
			{
				secureString.AppendChar(c);
			}

			return secureString;
		}

		/// <summary>
		/// 좌측에서 자르기
		/// </summary>
		/// <param name="source"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static string Left(this string source, int length)
		{
			if (source.Length > length)
				return source.Substring(0, length);
			else
				return source;
		}

		/// <summary>
		/// 우측에서 자르기
		/// </summary>
		/// <param name="source"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static string Right(this string source, int length)
		{
			if (source.Length > length)
				return source.Substring(source.Length - length, length);
			else
				return source;
		}

		/// <summary>
		/// 중간에서 자르기
		/// </summary>
		/// <param name="source"></param>
		/// <param name="startIndex"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static string Mid(this string source, int startIndex, int length)
		{
			if (source.Length > length && source.Length > startIndex)
				return source.Substring(startIndex, length);
			else
				return source;
		}

		/// <summary>
		/// 중간에서 자르기
		/// </summary>
		/// <param name="source"></param>
		/// <param name="startIndex"></param>
		/// <returns></returns>
		public static string Mid(this string source, int startIndex)
		{
			if (source.Length > startIndex)
				return source.Substring(startIndex);
			else
				return source;
		}

		/// <summary>
		/// 사업자 번호 검증
		/// </summary>
		/// <param name="businessNumber"></param>
		/// <returns></returns>
		public static bool IsValidBusinessNumber(this string businessNumber)
		{
			if (string.IsNullOrWhiteSpace(businessNumber)) return false;

			businessNumber = businessNumber.Replace(" ", "");
			businessNumber = businessNumber.Replace("-", "");

			if (businessNumber.Length != 10) return false;

			decimal parseOut = decimal.Zero;
			if (!decimal.TryParse(businessNumber, out parseOut)) return false;

			int nOut = 0;

			nOut += int.Parse(businessNumber.Substring(0, 1)) * 1;
			nOut += int.Parse(businessNumber.Substring(1, 1)) * 3;
			nOut += int.Parse(businessNumber.Substring(2, 1)) * 7;
			nOut += int.Parse(businessNumber.Substring(3, 1)) * 1;
			nOut += int.Parse(businessNumber.Substring(4, 1)) * 3;
			nOut += int.Parse(businessNumber.Substring(5, 1)) * 7;
			nOut += int.Parse(businessNumber.Substring(6, 1)) * 1;
			nOut += int.Parse(businessNumber.Substring(7, 1)) * 3;
			nOut += int.Parse(businessNumber.Substring(8, 1)) * 5 / 10;
			nOut += int.Parse(businessNumber.Substring(8, 1)) * 5 % 10;
			nOut += int.Parse(businessNumber.Substring(9, 1));

			if (nOut % 10 == 0) return true;
			else return false;
		}

		/// <summary>
		/// 주민등록번호 검증
		/// </summary>
		/// <param name="ssn"></param>
		/// <returns></returns>
		public static bool IsValidKoreanSocialSecurityNumber(this string ssn)
		{
			if (string.IsNullOrWhiteSpace(ssn)) return false;

			ssn = ssn.Replace(" ", "");
			ssn = ssn.Replace("-", "");

			if (ssn.Length != 13) return false;

			decimal parseOut = decimal.Zero;
			if (!decimal.TryParse(ssn, out parseOut)) return false;

			int sum = 0;

			for (int i = 0; i < ssn.Length - 1; i++)
			{
				char c = ssn[i];

				if (!char.IsNumber(c)) return false;
				else
				{
					if (i < ssn.Length)
					{
						sum += int.Parse(c.ToString()) * ((i % 8) + 2);
					}
				}
			}

			if (!((((11 - (sum % 11)) % 10).ToString()) == (ssn[ssn.Length - 1]).ToString()))
				return false;

			return true;
		}

		/// <summary>
		/// 외국인 등록번호 검증
		/// </summary>
		/// <param name="fSsn"></param>
		/// <returns></returns>
		public static bool IsValidForeignerNo(this string fSsn)
		{
			if (string.IsNullOrWhiteSpace(fSsn)) return false;

			fSsn = fSsn.Replace(" ", "");
			fSsn = fSsn.Replace("-", "");

			if (fSsn.Length != 13) return false;

			decimal parseOut = decimal.Zero;
			if (!decimal.TryParse(fSsn, out parseOut)) return false;

			if (int.Parse(fSsn.Substring(7, 2)) % 2 != 0) return false;

			int sum = 0;
			for (int i = 0; i < 12; i++)
			{
				sum += int.Parse(fSsn.Substring(i, 1)) * ((i % 8) + 2);
			}

			if ((((11 - (sum % 11)) % 10 + 2) % 10) == int.Parse(fSsn.Substring(12, 1))) return true;
			else return false;
		}

		/// <summary>
		/// eMail 주소 유효성 검증
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool IsValidEmailAddress(this string str)
		{
			Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
			return regex.IsMatch(str);
		}
	}
}