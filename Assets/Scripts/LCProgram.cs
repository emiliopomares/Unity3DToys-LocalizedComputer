using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class LCInstruction {
	public int opcode;
	public int dest;
	public int src1;
	public int src2;
	public float fimmediate;

	public LCInstruction(int o, int d, int s1, int s2, float f) {
		opcode = o;
		dest = d;
		src1 = s1;
		src2 = s2;
		fimmediate = f;
	}
}

public class LCProgram : MonoBehaviour {

	public InputField sourceEditor; // esto rompe un poco la abstracción, pero como esto no va a pasar de una demo, vale
	// esta clase no debería tener contacto con elementos de UI, debería ser puramente "blanca"

	public const int ldi = 0;
	public const int ldi_imm = 1;
	public const int iadd = 2;
	public const int iadd_imm = 3;
	public const int b = 4;
	public const int b_imm = 5;
	public const int bz = 6;
	public const int bz_imm = 7;
	public const int bnz = 8;
	public const int bnz_imm = 9;
	public const int icmp = 10;
	public const int icmp_imm = 11;
	public const int delay = 12;
	public const int halt = 13;

	public const int test_imm = 14; // see if there is adjacent unit (test top, test left, etc...) top = 1 right = 2 bottom = 3 left = 4 self = 0
	public const int bank_imm = 15; // change bank   bank top, bank self
	public const int lock_imm = 16; // lock top
	public const int unlock_imm = 17;

	public const int read_imm = 18;
	public const int write_imm = 19;
	public const int bgt = 20;
	public const int bgt_imm = 21;
	public const int isub = 22;
	public const int isub_imm = 23;
	public const int ldf = 24;
	public const int ldf_imm = 25;
	public const int fadd = 26;
	public const int fadd_imm = 27;
	public const int ldif = 28;
	public const int ldif_imm = 29;
	public const int ldfi = 30;
	public const int ldfi_imm = 31;
	public const int imul = 32;
	public const int imul_imm = 33;
	public const int fmul = 34;
	public const int fmul_imm = 35;


	public const int SelfBank = 0;
	public const int TopBank = 1;
	public const int RightBank = 2;
	public const int BottomBank = 3;
	public const int LeftBank = 4;




//	public const int imul = 0;
//	public const int idiv = 1;
//
//	public const int isub = 3;
//	public const int fmul = 4;
//	public const int fdiv = 5;
//	public const int fadd = 6;
//	public const int fsub = 7;
//	public const int cmp = 8;
//	public const int bz = 9;
//	public const int brz = 10;
//
//	public const int br = 12;
//
//	public const int ldf = 14;
//	public const int ld = 15;
//	public const int bgt = 16;
//	public const int blt = 17;
//	public const int bnz = 18;
//	public const int brnz = 19;


	public List<LCInstruction> compiledProgram;

	Dictionary<string, int> labels;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void getInstructionAt(int inst, out int op, out int p1, out int p2, out int p3, out float p4) {


		op = compiledProgram[inst].opcode;
		p1 = compiledProgram [inst].dest;
		p2 = compiledProgram [inst].src1;
		p3 = compiledProgram [inst].src2;
		p4 = compiledProgram [inst].fimmediate;

	}

	public void compileInputField() {
		compile (sourceEditor.text);
	}

	public bool compile(string program) {

		labels = new Dictionary<string, int> ();

		compiledProgram = new List<LCInstruction>();

		string[] instrs = program.Split ('\n');

		int currentPC = 0;

		for (int i = 0; i < instrs.Length; ++i) { // first, gather label offsets (first pass)
			if (instrs [i].EndsWith (":")) {

				string labelname = instrs [i].Substring (0, instrs [i].Length - 1);
				labels [labelname] = currentPC;

			} else {
				currentPC++;
			}
		}

		for (int i = 0; i < instrs.Length; ++i) { // then, instructions (second pass)




				string[] segments = instrs [i].Split (' ');

//			if (segments[0].Equals ("imul")) {
//
//				string[] arg = segments [1].Split (',');
//				int dest, src1, src2;
//				int.TryParse (arg [0], out dest);
//				int.TryParse (arg [1], out src1);
//				int.TryParse (arg [2], out src2);
//				compiledProgram.Add (imul);
//				compiledProgram.Add (dest);
//				compiledProgram.Add (src1);
//				compiledProgram.Add (src2);
//
//			}
//
//			if (segments[0].Equals ("idiv")) {
//
//				string[] arg = segments [1].Split (',');
//				int dest, src1, src2;
//				int.TryParse (arg [1], out dest);
//				int.TryParse (arg [2], out src1);
//				int.TryParse (arg [3], out src2);
//				compiledProgram.Add (idiv);
//				compiledProgram.Add (dest);
//				compiledProgram.Add (src1);
//				compiledProgram.Add (src2);
//
//			}
//
			if (segments[0].Equals ("add.i")) {

				string[] arg = segments [1].Split (',');
				int dest, src1, src2;
				int.TryParse (arg [0].Substring (1), out dest);
				int.TryParse (arg [1].Substring (1), out src1);

				bool imm = false;
				if (int.TryParse (arg [2], out src2)) {
					imm = true;
				} else
					int.TryParse (arg [2].Substring (1), out src2);

					if (imm)
						compiledProgram.Add (new LCInstruction (iadd_imm, dest, src1, src2, 0.0f));
					else
						compiledProgram.Add (new LCInstruction (iadd, dest, src1, src2, 0.0f));
				

			}

			if (segments[0].Equals ("add.f")) {

				string[] arg = segments [1].Split (',');
				int dest, src1, src2 = 0; float immval;
				int.TryParse (arg [0].Substring (1), out dest);
				int.TryParse (arg [1].Substring (1), out src1);

				bool imm = false;
				if (float.TryParse (arg [2], out immval)) {
					imm = true;
				} else
					int.TryParse (arg [2].Substring (1), out src2);

				if (imm)
					compiledProgram.Add (new LCInstruction (iadd_imm, dest, src1, 0, immval));
				else
					compiledProgram.Add (new LCInstruction (iadd, dest, src1, src2, 0.0f));


			}

			if (segments[0].Equals ("mul.i")) {

				string[] arg = segments [1].Split (',');
				int dest, src1, src2;
				int.TryParse (arg [0].Substring (1), out dest);
				int.TryParse (arg [1].Substring (1), out src1);

				bool imm = false;
				if (int.TryParse (arg [2], out src2)) {
					imm = true;
				} else
					int.TryParse (arg [2].Substring (1), out src2);

				if (imm)
					compiledProgram.Add (new LCInstruction (imul_imm, dest, src1, src2, 0.0f));
				else
					compiledProgram.Add (new LCInstruction (imul, dest, src1, src2, 0.0f));


			}

			if (segments[0].Equals ("mul.f")) {

				string[] arg = segments [1].Split (',');
				int dest, src1, src2 = 0; float immval;
				int.TryParse (arg [0].Substring (1), out dest);
				int.TryParse (arg [1].Substring (1), out src1);

				bool imm = false;
				if (float.TryParse (arg [2], out immval)) {
					imm = true;
				} else
					int.TryParse (arg [2].Substring (1), out src2);

				if (imm)
					compiledProgram.Add (new LCInstruction (imul_imm, dest, src1, 0, immval));
				else
					compiledProgram.Add (new LCInstruction (imul, dest, src1, src2, 0.0f));


			}

			if (segments[0].Equals ("sub.i")) {

				string[] arg = segments [1].Split (',');
				int dest, src1, src2;
				int.TryParse (arg [0].Substring (1), out dest);
				int.TryParse (arg [1].Substring (1), out src1);

				bool imm = false;
				if (int.TryParse (arg [2], out src2)) {
					imm = true;
				} else
					int.TryParse (arg [2].Substring (1), out src2);

				if (imm)
					compiledProgram.Add (new LCInstruction (isub_imm, dest, src1, src2, 0.0f));
				else
					compiledProgram.Add (new LCInstruction (isub, dest, src1, src2, 0.0f));


			}
//
//			if (segments[0].Equals ("isub")) {
//
//				string[] arg = segments [1].Split (',');
//				int dest, src1, src2;
//				int.TryParse (arg [1], out dest);
//				int.TryParse (arg [2], out src1);
//				int.TryParse (arg [3], out src2);
//				compiledProgram.Add (isub);
//				compiledProgram.Add (dest);
//				compiledProgram.Add (src1);
//				compiledProgram.Add (src2);
//
//			}
//
//			if (segments[0].Equals ("fmul")) {
//
//				string[] arg = segments [1].Split (',');
//				int dest, src1, src2;
//				int.TryParse (arg [1], out dest);
//				int.TryParse (arg [2], out src1);
//				int.TryParse (arg [3], out src2);
//				compiledProgram.Add (fmul);
//				compiledProgram.Add (dest);
//				compiledProgram.Add (src1);
//				compiledProgram.Add (src2);
//
//			}
//
//			if (segments[0].Equals ("fdiv")) {
//
//				string[] arg = segments [1].Split (',');
//				int dest, src1, src2;
//				int.TryParse (arg [1], out dest);
//				int.TryParse (arg [2], out src1);
//				int.TryParse (arg [3], out src2);
//				compiledProgram.Add (fdiv);
//				compiledProgram.Add (dest);
//				compiledProgram.Add (src1);
//				compiledProgram.Add (src2);
//
//			}
//
//			if (segments[0].Equals ("fadd")) {
//
//				string[] arg = segments [1].Split (',');
//				int dest, src1, src2;
//				int.TryParse (arg [1], out dest);
//				int.TryParse (arg [2], out src1);
//				int.TryParse (arg [3], out src2);
//				compiledProgram.Add (fadd);
//				compiledProgram.Add (dest);
//				compiledProgram.Add (src1);
//				compiledProgram.Add (src2);
//
//			}
//
//			if (segments[0].Equals ("fsub")) {
//
//				string[] arg = segments [1].Split (',');
//				int dest, src1, src2;
//				int.TryParse (arg [1], out dest);
//				int.TryParse (arg [2], out src1);
//				int.TryParse (arg [3], out src2);
//				compiledProgram.Add (fsub);
//				compiledProgram.Add (dest);
//				compiledProgram.Add (src1);
//				compiledProgram.Add (src2);
//
//			}
//
			if (segments[0].Equals ("cmp.i")) {

					string[] arg = segments [1].Split (',');
					int src1, src2;
					int.TryParse (arg [0].Substring (1), out src1);

					bool imm = false;
					if (int.TryParse (arg [1], out src2)) {
						imm = true;
					} else
						int.TryParse (arg [1].Substring (1), out src2);

					if (imm)
						compiledProgram.Add (new LCInstruction (icmp_imm, 0, src1, src2, 0.0f));
					else
						compiledProgram.Add (new LCInstruction (icmp, 0, src1, src2, 0.0f));

			}
//
//			if (segments[0].Equals ("bnz")) {
//
//				string[] arg = segments [1].Split (',');
//				int dest;
//				int.TryParse (arg [1], out dest);
//				compiledProgram.Add (bnz);
//				compiledProgram.Add (dest);
//				compiledProgram.Add (0);
//				compiledProgram.Add (0);
//
//			}
//
//			if (segments[0].Equals ("brnz")) {
//
//				string[] arg = segments [1].Split (',');
//				int dest;
//				int.TryParse (arg [1], out dest);
//				compiledProgram.Add (brnz);
//				compiledProgram.Add (dest);
//				compiledProgram.Add (0);
//				compiledProgram.Add (0);
//
//			}
//
//			if (segments[0].Equals ("bz")) {
//
//				string[] arg = segments [1].Split (',');
//				int dest;
//				int.TryParse (arg [1], out dest);
//				compiledProgram.Add (bz);
//				compiledProgram.Add (dest);
//				compiledProgram.Add (0);
//				compiledProgram.Add (0);
//
//			}
//
//			if (segments[0].Equals ("brz")) {
//
//				string[] arg = segments [1].Split (',');
//				int dest;
//				int.TryParse (arg [1], out dest);
//				compiledProgram.Add (bz);
//				compiledProgram.Add (dest);
//				compiledProgram.Add (0);
//				compiledProgram.Add (0);
//
//			}
//
				if (segments [0].Equals ("b")) {

					bool immediate = true;
					int iparam = 0;
					if((segments[1].Length > 1) && (int.TryParse(segments[1].Substring(1), out iparam)) && (segments[1].StartsWith("r"))) {
						immediate = false;
					}
					if (immediate)
						compiledProgram.Add (new LCInstruction (b_imm, labels [segments [1]], 0, 0, 0.0f));
					else
						compiledProgram.Add (new LCInstruction (b, iparam, 0, 0, 0.0f));

				}

				if (segments [0].Equals ("bz")) {

					bool immediate = true;
					int iparam = 0;
					if((segments[1].Length > 1) && (int.TryParse(segments[1].Substring(1), out iparam)) && (segments[1].StartsWith("r"))) {
						immediate = false;
					}
					if (immediate)
						compiledProgram.Add (new LCInstruction (bz_imm, labels [segments [1]], 0, 0, 0.0f));
					else
						compiledProgram.Add (new LCInstruction (bz, iparam, 0, 0, 0.0f));

				}

				if (segments [0].Equals ("bnz")) {

					bool immediate = true;
					int iparam = 0;
					if((segments[1].Length > 1) && (int.TryParse(segments[1].Substring(1), out iparam)) && (segments[1].StartsWith("r"))) {
						immediate = false;
					}
					if (immediate)
						compiledProgram.Add (new LCInstruction (bnz_imm, labels [segments [1]], 0, 0, 0.0f));
					else
						compiledProgram.Add (new LCInstruction (bnz, iparam, 0, 0, 0.0f));

				}

			if (segments [0].Equals ("bgt")) {

				bool immediate = true;
				int iparam = 0;
				if((segments[1].Length > 1) && (int.TryParse(segments[1].Substring(1), out iparam)) && (segments[1].StartsWith("r"))) {
					immediate = false;
				}
				if (immediate)
					compiledProgram.Add (new LCInstruction (bgt_imm, labels [segments [1]], 0, 0, 0.0f));
				else
					compiledProgram.Add (new LCInstruction (bgt, iparam, 0, 0, 0.0f));

			}
//
//			if (segments[0].Equals ("bgt")) {
//
//				string[] arg = segments [1].Split (',');
//				int dest;
//				int.TryParse (arg [1], out dest);
//				compiledProgram.Add (bgt);
//				compiledProgram.Add (dest);
//				compiledProgram.Add (0);
//				compiledProgram.Add (0);
//
//			}
//
//			if (segments[0].Equals ("blt")) {
//
//				string[] arg = segments [1].Split (',');
//				int dest;
//				int.TryParse (arg [1], out dest);
//				compiledProgram.Add (blt);
//				compiledProgram.Add (dest);
//				compiledProgram.Add (0);
//				compiledProgram.Add (0);
//
//			}
//
//			if (segments[0].Equals ("br")) {
//
//				string[] arg = segments [1].Split (',');
//				int dest;
//				int.TryParse (arg [1], out dest);
//				compiledProgram.Add (br);
//				compiledProgram.Add (dest);
//				compiledProgram.Add (0);
//				compiledProgram.Add (0);
//
//			}

			if (segments [0].Equals ("ld.if")) {

				string[] arg = segments [1].Split (',');
				int dest;
				int source = 0;
				float immval;
				int.TryParse (arg [0].Substring (1), out dest);
				bool imm = false;
				if (float.TryParse (arg [1], out immval)) {
					imm = true;
				} else
					int.TryParse (arg [1].Substring (1), out source);
				if (imm)
					compiledProgram.Add (new LCInstruction (ldif_imm, dest, 0, 0, immval));
				else
					compiledProgram.Add (new LCInstruction (ldif, dest, source, 0, 0.0f));

			}

			if (segments [0].Equals ("ld.fi")) {

				string[] arg = segments [1].Split (',');
				int dest;
				int source = 0;
				float immval;
				int.TryParse (arg [0].Substring (1), out dest);
				bool imm = false;
				if (float.TryParse (arg [1], out immval)) {
					imm = true;
				} else
					int.TryParse (arg [1].Substring (1), out source);
				if (imm)
					compiledProgram.Add (new LCInstruction (ldfi_imm, dest, 0, 0, immval));
				else
					compiledProgram.Add (new LCInstruction (ldfi, dest, source, 0, 0.0f));

			}


			if (segments [0].Equals ("ld.i")) {

				string[] arg = segments [1].Split (',');
				int dest;
				int source;
				int.TryParse (arg [0].Substring (1), out dest);
				bool imm = false;
				if (int.TryParse (arg [1], out source)) {
					imm = true;
				} else
					int.TryParse (arg [1].Substring (1), out source);
				if (imm)
					compiledProgram.Add (new LCInstruction (ldi_imm, dest, source, 0, 0.0f));
				else
					compiledProgram.Add (new LCInstruction (ldi, dest, source, 0, 0.0f));

			}

			if (segments [0].Equals ("ld.f")) {

				string[] arg = segments [1].Split (',');
				int dest;
				int source = 0;
				float immval;
				int.TryParse (arg [0].Substring (1), out dest);
				bool imm = false;
				if (float.TryParse (arg [1], out immval)) {
					imm = true;
				} else
					int.TryParse (arg [1].Substring (1), out source);
				if (imm)
					compiledProgram.Add (new LCInstruction (ldf_imm, dest, 0, 0, immval));
				else
					compiledProgram.Add (new LCInstruction (ldf, dest, source, 0, 0.0f));

			}


				if (segments [0].Equals ("test")) {

					int d = translateDirection (segments [1]);
					compiledProgram.Add (new LCInstruction (test_imm, d, 0, 0, 0.0f));

				}

			if (segments [0].Equals ("lock")) {

				int d = translateDirection (segments [1]);
				compiledProgram.Add (new LCInstruction (lock_imm, d, 0, 0, 0.0f));

			}

			if (segments [0].Equals ("unlock")) {

				compiledProgram.Add (new LCInstruction (unlock_imm, 0, 0, 0, 0.0f));

			}

			if (segments [0].Equals ("bank")) {

				int d = translateDirection (segments [1]);
				compiledProgram.Add (new LCInstruction (bank_imm, d, 0, 0, 0.0f));

			}

			if (segments [0].Equals ("read")) {

				int d = translateDirection (segments [1]);
				compiledProgram.Add (new LCInstruction (read_imm, d, 0, 0, 0.0f));

			}

			if (segments [0].Equals ("write")) {

				int d = translateDirection (segments [1]);
				compiledProgram.Add (new LCInstruction (write_imm, d, 0, 0, 0.0f));

			}
				
//
//			if (segments[0].Equals ("ld.f")) { // no sé qué hacer con esto...
//
//				string[] arg = segments [1].Split (',');
//				int dest;
//				float imm;
//				int.TryParse (arg [1], out dest);
//				float.TryParse (arg [2], out imm);
//				compiledProgram.Add (ldf);
//				compiledProgram.Add (dest);
//				compiledProgram.Add ((int)imm);
//				compiledProgram.Add (0);
//
//			}
//
//			if (segments [0].Equals ("ld.i")) {
//
//				string[] arg = segments [1].Split (',');
//				int dest;
//				int src;
//				int bank;
//				int.TryParse (arg [1], out dest);
//				int.TryParse (arg [2], out src);
//				int.TryParse (arg [3], out bank);
//				compiledProgram.Add (ldf);
//				compiledProgram.Add (dest);
//				compiledProgram.Add (src);
//				compiledProgram.Add (bank);
//			}

				if (segments [0].Equals ("delay")) {

					string[] arg = segments [1].Split (',');
					float time;
					float.TryParse (arg [0], out time);
					compiledProgram.Add (new LCInstruction (delay, 0, 0, 0, time));
				}

				if (segments [0].Equals ("halt")) { // halt and catch fire

					compiledProgram.Add (new LCInstruction (halt, 0, 0, 0, 0.0f));
			
				}


				




		}

		return true;


	}

	private int translateDirection(string dir) {
		if( dir=="top") 
			return TopBank;
		if (dir == "right")
			return RightBank;
		if (dir == "bottom")
			return BottomBank;
		if (dir == "left")
			return LeftBank;
		return 0;
	}
}
