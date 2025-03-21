using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class HomoglyphGraph {
	public static void Main() {
		Console.WriteLine("hello world!");
		HomoglyphGraph graph = new HomoglyphGraph();
		graph.CalculateAllPaths();
		char c = (char)0;
		char start = ' ';
		List<char> chars = new List<char>();
		while(c != 27) {
			c = Console.ReadKey().KeyChar;
			Console.Write("\b \b");
			if (c == 27) {
				break;
			}
			switch (c) {
				case '\b':
					if (chars.Count > 0) {
						chars.RemoveAt(chars.Count - 1);
					}
					break;
				case '\n':
				case '\r':
					for(int i = 0; i < chars.Count; ++i) {
						Console.Write(" ");
					}
					Console.Write("\r");
					PrintLettersMorphingFromOneToTheNext(chars, graph);
					Console.Write("\n");
					chars.Clear();
					break;
				default:
					chars.Add(c);
					break;
			}
			Console.Write($"\r{string.Join("", chars)}");
		}
	}

	static void PrintLettersMorphingFromOneToTheNext(List<char> chars, HomoglyphGraph graph) {
		char prevLetter = ' ';
		for(int i = 0; i < chars.Count; ++i) {
			graph.AnimateLetters(prevLetter, chars[i]);
			prevLetter = chars[i];
		}
	}

	void AnimateLetters(char start, char end) {
		string path = GetPathString(start, end);
		for (int i = 0; i < path.Length; ++i) {
			Console.Write($"{path[i]}");
			Thread.Sleep(100);
			if (i+1 < path.Length) {
				Console.Write("\b");
			}
		}
	}

	static string[] homoglyphs = {
" _",
"A4^HEVX",
"B813EP",
"Cc(<[{",
"D0OP)",
"E3BF",
"FETI",
"GCe6&0O",
"H#hA4B8KX",
"Il1|![])(][T",
"Jj7iL",
"KXHhY",
"L1i|",
"MmHNW",
"NMnWH",
"Oo0cCDQG@",
"PpqbR",
"QO0CDG@",
"RPpBbK",
"Ss5$Z",
"T+7F",
"UuVvY",
"VUuYyWH",
"WwVv",
"XxY",
"YVXy",
"Z2z7",
"a@d",
"bdp6oh",
"c(C<oer",
"dqb9",
"eEco",
"ft+F",
"g9q",
"hHkb",
"il1|!:;j",
"jiJ;",
"kKHXhb",
"l1|I!",
"mnMN",
"nmMNur",
"o0Ocpdeu",
"pbdoq",
"qpdg",
"rcn",
"s5$z",
"t+Tf",
"uUvVno",
"vVuYy",
"wWVv",
"xX+vk*",
"yYVvu",
"zZ2s",
"0OQCDG@",
"1lI|!",
"2Zz7",
"3E8B",
"4A^H",
"5S$",
"6Gb",
"7Tt?Z",
"8B3HS",
"9gq",
"@aQ0O",
"#H",
"$S5",
"%96o/",
"^A4",
"&Gg",
"*x+'",
"+xX*tf",
"-_=",
"_- ",
"=_e#c",
"|l1I!][/\\",
"\\/|",
"/\\|%",
"`'",
"'`\"",
"\"',",
";:i,",
":;i.",
"!:;|i",
",. ",
".,",
"<([{",
">)]}",
"({[<",
")]}>",
"{[(",
"}])",
"[{(",
"]})",
"?7",
		};

	Dictionary<char, byte> indexOf = new Dictionary<char, byte>();

	private void Init() {
		for (byte i = 0; i < homoglyphs.Length; i++) {
			indexOf[homoglyphs[i][0]] = i;
		}
	}

	string GetChildren(char c) {
		if (!indexOf.TryGetValue(c, out byte index)) {
			return null;
		}
		return homoglyphs[index].Substring(1);
	}

	private byte[][] allPaths;
	const byte UNDEFINED = 255;

	public string GetPathString(char start, char end) {
		char[] p = GetPath(start, end);
		if (p == null) { return null; }
		return new string(p);
	}

	public char[] GetPath(char start, char target) {
		List<byte> path = new List<byte>();
		if (!indexOf.TryGetValue(start, out byte s) || !indexOf.TryGetValue(target, out byte u)) {
			return null;
		}
		byte[] prev = allPaths[s];
		if (prev[u] != UNDEFINED || u == s) {
			while (u != UNDEFINED) {
				path.Add(u);
				u = prev[u];
			}
		}
		char[] result = new char[path.Count];
		for(int i = 0; i < path.Count; ++i) {
			result[i] = homoglyphs[path[path.Count - 1 - i]][0];
		}
		return result;
	}

	public void CalculateAllPaths() {
		Init();
		allPaths = new byte[homoglyphs.Length][];
		for (byte i = 0; i < allPaths.Length; ++i) {
			byte[] paths = Djikstras(i);
			//Console.WriteLine(string.Join(' ', paths));
			allPaths[i] = paths;
		}
	}

	public byte[] Djikstras(byte source) {
		int[] dist = new int[homoglyphs.Length];
		byte[] prev = new byte[homoglyphs.Length];
		List<byte> queue = new List<byte>();
		for (int i = 0; i < homoglyphs.Length; ++i) {
			dist[i] = -1;
			prev[i] = UNDEFINED;
			queue.Add((byte)i);
		}
		//prev[source] = source;
		dist[source] = 0;
		while (queue.Count > 0) {
			byte u = minDist();
			if (!queue.Remove(u)) {
				for(int i = 0; i < queue.Count; ++i) {
					char c = homoglyphs[queue[i]][0];
					int d = dist[queue[i]];
				}
				throw new Exception($"we have a problem. {u} was not removed:\n{string.Join(' ', queue)}");
			}
			char self = homoglyphs[u][0];
			string neighbors = homoglyphs[u].Substring(1);
			//Console.WriteLine($"{u}  {self}: {neighbors}");
			for (int i = 0; i < neighbors.Length; ++i) {
				if (!indexOf.TryGetValue(neighbors[i], out byte v)) {
					throw new Exception($"missing entry for {v} '{neighbors[i]}'");
				}
				int alt = dist[u] + 1;
				//Console.WriteLine($"{v}  '{homoglyphs[v][0]}'    {alt} vs {dist[v]}");
				if (dist[v] < 0 || alt < dist[v]) {
					dist[v] = alt;
					prev[v] = u;
				}
			}
			//Console.ReadKey();
		}
		return prev;
		byte minDist() {
			int bestDist = -1;
			byte bestVertex = 0;
			foreach (byte a in queue) {
				int d = dist[a];
				if (d >= 0 && (d < bestDist || bestDist < 0)) {
					bestVertex = a;
					bestDist = d;
				}
			}
			return bestVertex;
		}
	}
}
