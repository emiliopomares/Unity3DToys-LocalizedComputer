using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LCProcessingUnit : MonoBehaviour {

	const int MaxiMemory = 64;

	public LCProgram program;

	public UnitCell physicalCell;

	public LCProcessingUnit topUnit;
	public LCProcessingUnit bottomUnit;
	public LCProcessingUnit leftUnit;
	public LCProcessingUnit rightUnit;

	LCProcessingUnit XPlus;
	LCProcessingUnit XMinus;
	LCProcessingUnit YPlus;
	LCProcessingUnit YMinus;

	// bank: 16 bits (dest)   16 bits (src)
	// 
	const int THISCELL = 0;
	const int LEFTXCELL = 1;
	const int RIGHTXCELL = 2;
	const int DOWNYCELL = 3;
	const int UPYCELL = 4;
	const int DOWNZCELL = 5;
	const int UPZCELL = 6;


	List<System.Action<int, int, int, float>> opcodes;

	public int initialR0;

	public int[] iMemory;
	public float[] fMemory;
	bool Zero;
	bool GreaterThan;
	bool LowerThan;
	bool Carry;
	bool Fail;

	int bank;
	int readbank;
	int writebank;

	bool halted = true;

	int lockedbank = -1;

	bool locked = false;

	public int PC = 0;

	float remainingDelayTime = 0.0f;

	// src1 is the integer immediate in immediate ops

	void imul_execute(int dest, int src1, int src2, float fimmediate) {

		iMemory [dest] = iMemory [src1] * iMemory [src2];
		Zero = (iMemory [dest] == 0);

	}
	void imul_immediate_execute(int dest, int src1, int src2, float fimmediate) {

		iMemory [dest] = iMemory [src1] * src2;
		Zero = (iMemory [dest] == 0);

	}
	void idiv_execute(int dest, int src1, int src2, float fimmediate) {

		iMemory [dest] = iMemory [src1] / iMemory [src2];

	}
	void iadd_execute(int dest, int src1, int src2, float fimmediate) {

		iMemory [dest] = iMemory [src1] + iMemory [src2];
		Zero = (iMemory [dest] == 0);

	}
	void iadd_immediate_execute(int dest, int src1, int src2, float fimmediate) {

		iMemory [dest] = iMemory [src1] + src2;
		Zero = (iMemory [dest] == 0);

	}
	void isub_execute(int dest, int src1, int src2, float fimmediate) {

		iMemory [dest] = iMemory [src1] - iMemory [src2];
		Zero = (iMemory [dest] == 0);

	}
	void isub_immediate_execute(int dest, int src1, int src2, float fimmediate) {

		iMemory [dest] = iMemory [src1] - src2;
		Zero = (iMemory [dest] == 0);

	}
	void fmul_execute(int dest, int src1, int src2, float fimmediate) {

		fMemory [dest] = fMemory [src1] * fMemory [src2];
		Zero = (fMemory [dest] == 0.0f);

	}
	void fmul_immediate_execute(int dest, int src1, int src2, float fimmediate) {

		fMemory [dest] = fMemory [src1] * fimmediate;
		Zero = (fMemory [dest] == 0.0f);

	}
	void fdiv_execute(int dest, int src1, int src2, float fimmediate) {

		iMemory [dest] = iMemory [src1] / iMemory [src2];

	}
	void fadd_execute(int dest, int src1, int src2, float fimmediate) {

		fMemory [dest] = fMemory [src1] + fMemory [src2];
		Zero = (fMemory [dest] == 0.0f);

	}
	void fadd_immediate_execute(int dest, int src1, int src2, float fimmediate) {

		fMemory [dest] = fMemory [src1] + fimmediate;
		Zero = (fMemory [dest] == 0.0f);

	}
	void bank_immediate_execute(int dest, int src1, int src2, float fimmediate) {
		bank = dest;
	}
	void read_immediate_execute(int dest, int src1, int src2, float fimmediate) {
		readbank = dest;
	}
	void write_immediate_execute(int dest, int src1, int src2, float fimmediate) {
		writebank = dest;
	}
	void fsub_execute(int dest, int src1, int src2, float fimmediate) {

		iMemory [dest] = iMemory [src1] - iMemory [src2];
		Zero = (iMemory [dest] == 0);

	}
	void cmpi_execute(int dest, int src1, int src2, float fimmediate) { 
		Zero = (iMemory [src1] == iMemory [src2]);
		GreaterThan = (iMemory [src1] > iMemory [src2]);
		LowerThan = (iMemory [src1] < iMemory [src2]);
	}
	void test_immediate_execute(int dest, int src1, int src2, float fimmediate) {
		if (dest == LCProgram.TopBank)
			Zero = (topUnit != null);
		if (dest == LCProgram.RightBank)
			Zero = (rightUnit != null);
		if (dest == LCProgram.BottomBank)
			Zero = (bottomUnit != null);
		if (dest == LCProgram.LeftBank)
			Zero = (leftUnit != null);
	}
	void cmpi_immediate_execute(int dest, int src1, int src2, float fimmediate) {
		Zero = (iMemory [src1] == src2);
		GreaterThan = (iMemory [src1] > src2);
		LowerThan = (iMemory [src1] < src2);
	}
	void bz_execute(int dest, int src1, int src2, float fimmediate) {
		if (Zero)
			PC = iMemory[dest];
	}
	void bz_immediate_execute(int dest, int src1, int src2, float fimmediate) {
		if (Zero)
			PC = dest;
	}
	void bnz_execute(int dest, int src1, int src2, float fimmediate) {
		if (!Zero)
			PC = iMemory[dest];
	}
	void bnz_immediate_execute(int dest, int src1, int src2, float fimmediate) {
		if (!Zero)
			PC = dest;
	}
	void bgt_execute(int dest, int src1, int src2, float fimmediate) {
		if (GreaterThan)
			PC = iMemory[dest];
	}
	void bgt_immediate_execute(int dest, int src1, int src2, float fimmediate) {
		if (GreaterThan)
			PC = dest;
	}
	void b_execute(int dest, int src1, int src2, float fimmediate) {
		PC = iMemory[dest];
	}
	void b_immediate_execute(int dest, int src1, int src2, float fimmediate) {
		PC = dest;
	}
	void brz_execute(int dest, int src1, int src2, float fimmediate) {
		if (Zero)
			PC += dest;
	}
	void brnz_execute(int dest, int src1, int src2, float fimmediate) {
		if (!Zero)
			PC += dest;
	}
	void ldi_execute(int dest, int src1, int src2, float fimmediate) { // bank
		int rdata = 0;
		if (readbank == LCProgram.SelfBank) {
			rdata = iMemory [src1];
			Fail = false;
		} else if (readbank == LCProgram.RightBank) {
			if (rightUnit != null) {
				rdata = rightUnit.iMemory [src1];
				Fail = false;
			} else
				Fail = true;
		} else if (readbank == LCProgram.TopBank) {
			if (topUnit != null) {
				rdata = topUnit.iMemory [src1];
				Fail = false;
			} else
				Fail = true;
		} else if (readbank == LCProgram.LeftBank) {
			if (leftUnit != null) {
				rdata = leftUnit.iMemory [src1];
				Fail = false;
			} else
				Fail = true;
		} else if (readbank == LCProgram.BottomBank) {
			if (bottomUnit != null) {
				rdata = bottomUnit.iMemory [src1];
				Fail = false;
			} else
				Fail = true;
		}
		if (Fail)
			return;
		if (writebank == LCProgram.SelfBank) {
			iMemory [dest] = rdata;
			Fail = false;
		} else if (writebank == LCProgram.RightBank) {
			if (rightUnit != null) {
				rightUnit.iMemory [dest] = rdata;
				Fail = false;
			} else
				Fail = true;
		} else if (writebank == LCProgram.TopBank) {
			if (topUnit != null) {
				topUnit.iMemory [dest] = rdata;
				Fail = false;
			} else
				Fail = true;
		} else if (writebank == LCProgram.LeftBank) {
			if (leftUnit != null) {
				leftUnit.iMemory [dest] = rdata;
				Fail = false;
			} else
				Fail = true;
		} else if (writebank == LCProgram.BottomBank) {
			if (bottomUnit != null) {
				bottomUnit.iMemory [dest] = rdata;
				Fail = false;
			} else
				Fail = true;
		}
		if(!Fail) Zero = (rdata == 0);
	}
	void ldfi_execute(int dest, int src1, int src2, float fimmediate) { // bank
		int rdata = 0;
		if (readbank == LCProgram.SelfBank) {
			rdata = iMemory [src1];
			Fail = false;
		} else if (readbank == LCProgram.RightBank) {
			if (rightUnit != null) {
				rdata = rightUnit.iMemory [src1];
				Fail = false;
			} else
				Fail = true;
		} else if (readbank == LCProgram.TopBank) {
			if (topUnit != null) {
				rdata = topUnit.iMemory [src1];
				Fail = false;
			} else
				Fail = true;
		} else if (readbank == LCProgram.LeftBank) {
			if (leftUnit != null) {
				rdata = leftUnit.iMemory [src1];
				Fail = false;
			} else
				Fail = true;
		} else if (readbank == LCProgram.BottomBank) {
			if (bottomUnit != null) {
				rdata = bottomUnit.iMemory [src1];
				Fail = false;
			} else
				Fail = true;
		}
		if (Fail)
			return;
		if (writebank == LCProgram.SelfBank) {
			fMemory [dest] = (float)rdata;
			Fail = false;
		} else if (writebank == LCProgram.RightBank) {
			if (rightUnit != null) {
				rightUnit.fMemory [dest] = (float)rdata;
				Fail = false;
			} else
				Fail = true;
		} else if (writebank == LCProgram.TopBank) {
			if (topUnit != null) {
				topUnit.fMemory [dest] = (float)rdata;
				Fail = false;
			} else
				Fail = true;
		} else if (writebank == LCProgram.LeftBank) {
			if (leftUnit != null) {
				leftUnit.fMemory [dest] = (float)rdata;
				Fail = false;
			} else
				Fail = true;
		} else if (writebank == LCProgram.BottomBank) {
			if (bottomUnit != null) {
				bottomUnit.fMemory [dest] = (float)rdata;
				Fail = false;
			} else
				Fail = true;
		}
		if(!Fail) Zero = (rdata == 0);
	}
	void ldif_execute(int dest, int src1, int src2, float fimmediate) { // bank
		float rdata = 0;
		if (readbank == LCProgram.SelfBank) {
			rdata = fMemory [src1];
			Fail = false;
		} else if (readbank == LCProgram.RightBank) {
			if (rightUnit != null) {
				rdata = rightUnit.fMemory [src1];
				Fail = false;
			} else
				Fail = true;
		} else if (readbank == LCProgram.TopBank) {
			if (topUnit != null) {
				rdata = topUnit.fMemory [src1];
				Fail = false;
			} else
				Fail = true;
		} else if (readbank == LCProgram.LeftBank) {
			if (leftUnit != null) {
				rdata = leftUnit.fMemory [src1];
				Fail = false;
			} else
				Fail = true;
		} else if (readbank == LCProgram.BottomBank) {
			if (bottomUnit != null) {
				rdata = bottomUnit.fMemory [src1];
				Fail = false;
			} else
				Fail = true;
		}
		if (Fail)
			return;
		if (writebank == LCProgram.SelfBank) {
			iMemory [dest] = (int)rdata;
			Fail = false;
		} else if (writebank == LCProgram.RightBank) {
			if (rightUnit != null) {
				rightUnit.iMemory [dest] = (int)rdata;
				Fail = false;
			} else
				Fail = true;
		} else if (writebank == LCProgram.TopBank) {
			if (topUnit != null) {
				topUnit.iMemory [dest] = (int)rdata;
				Fail = false;
			} else
				Fail = true;
		} else if (writebank == LCProgram.LeftBank) {
			if (leftUnit != null) {
				leftUnit.iMemory [dest] = (int)rdata;
				Fail = false;
			} else
				Fail = true;
		} else if (writebank == LCProgram.BottomBank) {
			if (bottomUnit != null) {
				bottomUnit.iMemory [dest] = (int)rdata;
				Fail = false;
			} else
				Fail = true;
		}
		if(!Fail) Zero = (rdata == 0);
	}
	void ldf_execute(int dest, int src1, int src2, float fimmediate) { // bank
		float rdata = 0.0f;
		if (readbank == LCProgram.SelfBank) {
			rdata = fMemory [src1];
			Fail = false;
		} else if (readbank == LCProgram.RightBank) {
			if (rightUnit != null) {
				rdata = rightUnit.fMemory [src1];
				Fail = false;
			} else
				Fail = true;
		} else if (readbank == LCProgram.TopBank) {
			if (topUnit != null) {
				rdata = topUnit.fMemory [src1];
				Fail = false;
			} else
				Fail = true;
		} else if (readbank == LCProgram.LeftBank) {
			if (leftUnit != null) {
				rdata = leftUnit.fMemory [src1];
				Fail = false;
			} else
				Fail = true;
		} else if (readbank == LCProgram.BottomBank) {
			if (bottomUnit != null) {
				rdata = bottomUnit.fMemory [src1];
				Fail = false;
			} else
				Fail = true;
		}
		if (Fail)
			return;
		if (writebank == LCProgram.SelfBank) {
			fMemory [dest] = rdata;
			Fail = false;
		} else if (writebank == LCProgram.RightBank) {
			if (rightUnit != null) {
				rightUnit.fMemory [dest] = rdata;
				Fail = false;
			} else
				Fail = true;
		} else if (writebank == LCProgram.TopBank) {
			if (topUnit != null) {
				topUnit.fMemory [dest] = rdata;
				Fail = false;
			} else
				Fail = true;
		} else if (writebank == LCProgram.LeftBank) {
			if (leftUnit != null) {
				leftUnit.fMemory [dest] = rdata;
				Fail = false;
			} else
				Fail = true;
		} else if (writebank == LCProgram.BottomBank) {
			if (bottomUnit != null) {
				bottomUnit.fMemory [dest] = rdata;
				Fail = false;
			} else
				Fail = true;
		}
		if(!Fail) Zero = (rdata == 0.0f);
	}
	void ldi_immediate_execute(int dest, int src1, int src2, float fimmediate) {

		if (writebank == LCProgram.SelfBank) {
			iMemory [dest] = src1;
			Fail = false;
		} else if (writebank == LCProgram.RightBank) {
			if (rightUnit != null) {
				rightUnit.iMemory [dest] = src1;
				Fail = false;
			} else
				Fail = true;
		} else if (writebank == LCProgram.TopBank) {
			if (topUnit != null) {
				topUnit.iMemory [dest] = src1;
				Fail = false;
			} else
				Fail = true;
		} else if (writebank == LCProgram.LeftBank) {
			if (leftUnit != null) {
				leftUnit.iMemory [dest] = src1;
				Fail = false;
			} else
				Fail = true;
		} else if (writebank == LCProgram.BottomBank) {
			if (bottomUnit != null) {
				bottomUnit.iMemory [dest] = src1;
				Fail = false;
			} else
				Fail = true;
		}
		if(!Fail) Zero = (src1 == 0);

	}
	void ldfi_immediate_execute(int dest, int src1, int src2, float fimmediate) {

		if (writebank == LCProgram.SelfBank) {
			fMemory [dest] = (float)src1;
			Fail = false;
		} else if (writebank == LCProgram.RightBank) {
			if (rightUnit != null) {
				rightUnit.fMemory [dest] = (float)src1;
				Fail = false;
			} else
				Fail = true;
		} else if (writebank == LCProgram.TopBank) {
			if (topUnit != null) {
				topUnit.fMemory [dest] = (float)src1;
				Fail = false;
			} else
				Fail = true;
		} else if (writebank == LCProgram.LeftBank) {
			if (leftUnit != null) {
				leftUnit.fMemory [dest] = (float)src1;
				Fail = false;
			} else
				Fail = true;
		} else if (writebank == LCProgram.BottomBank) {
			if (bottomUnit != null) {
				bottomUnit.fMemory [dest] = (float)src1;
				Fail = false;
			} else
				Fail = true;
		}
		if(!Fail) Zero = (src1 == 0);

	}
	void ldif_immediate_execute(int dest, int src1, int src2, float fimmediate) {

		if (writebank == LCProgram.SelfBank) {
			iMemory [dest] = (int)fimmediate;
			Fail = false;
		} else if (writebank == LCProgram.RightBank) {
			if (rightUnit != null) {
				rightUnit.iMemory [dest] = (int)fimmediate;
				Fail = false;
			} else
				Fail = true;
		} else if (writebank == LCProgram.TopBank) {
			if (topUnit != null) {
				topUnit.iMemory [dest] = (int)fimmediate;
				Fail = false;
			} else
				Fail = true;
		} else if (writebank == LCProgram.LeftBank) {
			if (leftUnit != null) {
				leftUnit.iMemory [dest] = (int)fimmediate;
				Fail = false;
			} else
				Fail = true;
		} else if (writebank == LCProgram.BottomBank) {
			if (bottomUnit != null) {
				bottomUnit.iMemory [dest] = (int)fimmediate;
				Fail = false;
			} else
				Fail = true;
		}
		if(!Fail) Zero = (src1 == 0);

	}
	void ldf_immediate_execute(int dest, int src1, int src2, float fimmediate) {

		if (writebank == LCProgram.SelfBank) {
			fMemory [dest] = fimmediate;
			Fail = false;
		} else if (writebank == LCProgram.RightBank) {
			if (rightUnit != null) {
				rightUnit.fMemory [dest] = fimmediate;
				Fail = false;
			} else
				Fail = true;
		} else if (writebank == LCProgram.TopBank) {
			if (topUnit != null) {
				topUnit.fMemory [dest] = fimmediate;
				Fail = false;
			} else
				Fail = true;
		} else if (writebank == LCProgram.LeftBank) {
			if (leftUnit != null) {
				leftUnit.fMemory [dest] = fimmediate;
				Fail = false;
			} else
				Fail = true;
		} else if (writebank == LCProgram.BottomBank) {
			if (bottomUnit != null) {
				bottomUnit.fMemory [dest] = fimmediate;
				Fail = false;
			} else
				Fail = true;
		}
		if(!Fail) Zero = (fimmediate == 0.0f);

	}
	void halt_execute(int dest, int src1, int src2, float fimmediate) {
		unlock_immediate_execute (0, 0, 0, 0.0f);
		setRunningState (false);
	}
	void delay_execute(int dest, int src1, int src2, float fimmediate) {
		remainingDelayTime = fimmediate;
	}
	void lock_immediate_execute(int dest, int src1, int src2, float fimmediate) {
		locked = true;
		if (dest == LCProgram.BottomBank) {
			if (bottomUnit != null) {
				if (bottomUnit.locked == false) {
					bottomUnit.halted = true;
					bottomUnit.locked = true;
					lockedbank = LCProgram.BottomBank;
					Zero = true;
				} else
					Zero = false;
			} else
				Zero = false;
		}
		if (dest == LCProgram.TopBank) {
			if (topUnit != null) {
				if (topUnit.locked == false) {
					topUnit.halted = true;
					topUnit.locked = true;
					lockedbank = LCProgram.TopBank;
					Zero = true;
				} else
					Zero = false;
			} else
				Zero = false;
		}
		if (dest == LCProgram.LeftBank) {
			if (leftUnit != null) {
				if (leftUnit.locked == false) {
					leftUnit.halted = true;
					leftUnit.locked = true;
					lockedbank = LCProgram.LeftBank;
					Zero = true;
				} else
					Zero = false;
			} else
				Zero = false;
		}
		if (dest == LCProgram.RightBank) {
			if (rightUnit != null) {
				if (rightUnit.locked == false) {
					rightUnit.halted = true;
					rightUnit.locked = true;
					lockedbank = LCProgram.RightBank;
					Zero = true;
				} else
					Zero = false;
			} else
				Zero = false;
		}
		locked = Zero;
	}
	void unlock_immediate_execute(int dest, int src1, int src2, float fimmediate) {
		if (lockedbank == LCProgram.BottomBank) {
			bottomUnit.locked = false;
			bottomUnit.halted = false;
		}
		if (lockedbank == LCProgram.TopBank) {
			topUnit.locked = false;
			topUnit.halted = false;
		}
		if (lockedbank == LCProgram.LeftBank) {
			leftUnit.locked = false;
			leftUnit.halted = false;
		}
		if (lockedbank == LCProgram.RightBank) {
			rightUnit.locked = false;
			rightUnit.halted = false;
		}
		lockedbank = -1;
		locked = false;
	}
	void ld_execute(int dest, int immediate, int bank) {
		LCProcessingUnit destUnit=null, srcUnit=null;
		int destBank = (bank >> 16) & 0xFFFF;
		int srcBank = bank & 0xFFFF;
		switch (destBank) {
		case THISCELL:
			destUnit = this;
			break;
		case LEFTXCELL:
			destUnit = XMinus;
			break;
		case RIGHTXCELL:
			destUnit = XPlus;
			break;
		case DOWNYCELL:
			destUnit = YMinus;
			break;
		case UPYCELL:
			destUnit = YPlus;
			break;
		}
		switch (srcBank) {
		case THISCELL:
			srcUnit = this;
			break;
		case LEFTXCELL:
			srcUnit = XMinus;
			break;
		case RIGHTXCELL:
			srcUnit = XPlus;
			break;
		case DOWNYCELL:
			srcUnit = YMinus;
			break;
		case UPYCELL:
			srcUnit = YPlus;
			break;
		}
		if (destUnit == null)
			return;
		int readValue;
		if (srcUnit == null)
			readValue = 0;
		else
			readValue = srcUnit.readiMemoryAt (immediate);
		destUnit.writeiMemoryAt (immediate, readValue);
	}

	public void writeiMemoryAt(int address, int value) {
		iMemory [address] = value;
	}

	public int readiMemoryAt(int address) {
		return iMemory [address];
	}

	public void setRunningState(bool running) {
		halted = !running;
		locked = !running;
		physicalCell.setStatusLED (running);
	}

	public void reset() {
		for (int i = 0; i < iMemory.Length; ++i) {
			iMemory [i] = 0;
		}
		for (int i = 0; i < fMemory.Length; ++i) {
			fMemory [i] = 0.0f;
		}
		PC = 0;
	}

	// Use this for initialization
	void Start () {
		setRunningState (false);
		PC = 0;
		locked = false;
		lockedbank = -1;
		bank = LCProgram.SelfBank;
		iMemory = new int[MaxiMemory];
		fMemory = new float[MaxiMemory];
		iMemory [0] = initialR0;
		physicalCell.value = iMemory [0];
			
			opcodes = new List<System.Action<int, int, int, float>> ();
			opcodes.Add (ldi_execute);
			opcodes.Add (ldi_immediate_execute);
			opcodes.Add (iadd_execute);
			opcodes.Add (iadd_immediate_execute);
			opcodes.Add (b_execute);
			opcodes.Add (b_immediate_execute);
			opcodes.Add (bz_execute);
			opcodes.Add (bz_immediate_execute);
			opcodes.Add (bnz_execute);
			opcodes.Add (bnz_immediate_execute);
			opcodes.Add (cmpi_execute);
			opcodes.Add (cmpi_immediate_execute);
			opcodes.Add (delay_execute);
			opcodes.Add (halt_execute);
			opcodes.Add (test_immediate_execute);
			opcodes.Add (bank_immediate_execute);
			opcodes.Add (lock_immediate_execute);
			opcodes.Add (unlock_immediate_execute);
			opcodes.Add (read_immediate_execute);
			opcodes.Add (write_immediate_execute);
			opcodes.Add (bgt_execute);
			opcodes.Add (bgt_immediate_execute);
			opcodes.Add (isub_execute);
			opcodes.Add (isub_immediate_execute);
			opcodes.Add (ldf_execute);
			opcodes.Add (ldf_immediate_execute);
			opcodes.Add (fadd_execute);
			opcodes.Add (fadd_immediate_execute);
			opcodes.Add (ldif_execute);
			opcodes.Add (ldif_immediate_execute);
			opcodes.Add (ldfi_execute);
			opcodes.Add (ldfi_immediate_execute);
			opcodes.Add (imul_execute);
			opcodes.Add (imul_immediate_execute);
			opcodes.Add (fmul_execute);
			opcodes.Add (fmul_immediate_execute);
		/*
		 * number of companions:
		 * 
		 * ld.i r0,0
		 * test top
		 * jnz notop
		 * add.i r0,r0,1
		 * notop:
		 * test left
		 * jnz noleft
		 * add.i r0,r0,1
		 * noleft:
		 * test right
		 * jnz noright
		 * add.i r0,r0,1
		 * noright:
		 * test bottom
		 * jnz nobottom
		 * add.i r0,r0,1
		 * nobottom:
		 * halt
		 * 
		 */



	}

	public void attachProgram(LCProgram prog) {
		program = prog;
	}
	
	// Update is called once per frame
	void Update () {

		if (program == null)
			return;

		if (!halted) {

			if (remainingDelayTime > 0.0f) {
				remainingDelayTime -= Time.deltaTime;
			} else {

				// process a single instruction
				int opcode, param1, param2, param3;
				float param4;
				if (program != null) {
					program.getInstructionAt (PC, out opcode, out param1, out param2, out param3, out param4);
					int prevPC = PC;
					opcodes [opcode] (param1, param2, param3, param4);
					if (PC == prevPC)
						++PC;
				}
				physicalCell.value = iMemory [0];

			}
		}



	}



//	public void compile(string program) {
//
//		string[] lines = program.Split ('\n');
//		for (int i = 0; i < lines.Length; ++i) {
//			string[] segments = lines [i].Split (' ');
//
//		}
//
//	}
}
