
using System;
using System.Collections.Generic;

namespace SoulSmithEmotions
{
	//Is the order of the emotions, represented as binary strings FATWDJ
	public enum EmotionTag
	{
		Typeless =		0b000000,
		Joy =			0b000001, //Singles
		Despair =		0b000010,
		Wrath =			0b000100, 
		Tranquility =	0b001000,
		Adoration =		0b010000,
		Fear =			0b100000,
		Poignancy =		0b000011, //Twofers, joy
		Catharsis =		0b000101,
		Comfort =		0b001001,
		Affection =		0b010001,
		Exhilaration =	0b100001,
		Bitterness =	0b000110, //Despair
		Misery =		0b001010,
		Loss =			0b010010,
		Shame =			0b100010,
		Ego =			0b001100, //Wrath
		Ambivalence =	0b010100,
		Angst =			0b100100,
		Loyalty =		0b011000, //Tranquility
		Dread =			0b101000,
		Respect =		0b110000, //Adoration
        Eraticism =		0b000111, //Threefers, joy
		Nostalgia =		0b001011,
		Pride =			0b001101,
		Salvation =		0b010011,
        Passion =		0b010101,
		Devotion =		0b011001,
		Denial =		0b100011,
		Surprisal =		0b100101,
		Excitement =	0b101001,
		Tension =		0b110001,
		Regret =		0b001110, //Despair
		Rejection =		0b010110,
		Longing =		0b011010,
		Panic =			0b100110,
        Acquiescence =	0b101010,
		Timidity =		0b110010,
        Discipline =	0b011100, //Wrath
		Anguish =		0b101100,
		Betrayal =		0b110100, 
		Servitude =		0b111000, //Tranquility

		//Contempt =	   0b1000000,
		
    }


}

