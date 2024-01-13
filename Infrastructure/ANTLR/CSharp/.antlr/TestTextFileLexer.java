// Generated from c:/Users/USUARIO/source/repos/FlowUML/Infrastructure/ANTLR/CSharp/TestTextFile.g4 by ANTLR 4.13.1
import org.antlr.v4.runtime.Lexer;
import org.antlr.v4.runtime.CharStream;
import org.antlr.v4.runtime.Token;
import org.antlr.v4.runtime.TokenStream;
import org.antlr.v4.runtime.*;
import org.antlr.v4.runtime.atn.*;
import org.antlr.v4.runtime.dfa.DFA;
import org.antlr.v4.runtime.misc.*;

@SuppressWarnings({"all", "warnings", "unchecked", "unused", "cast", "CheckReturnValue", "this-escape"})
public class TestTextFileLexer extends Lexer {
	static { RuntimeMetaData.checkVersion("4.13.1", RuntimeMetaData.VERSION); }

	protected static final DFA[] _decisionToDFA;
	protected static final PredictionContextCache _sharedContextCache =
		new PredictionContextCache();
	public static final int
		T__0=1, T__1=2, T__2=3, T__3=4, T__4=5, T__5=6, T__6=7, T__7=8, T__8=9, 
		T__9=10, T__10=11, T__11=12, T__12=13, T__13=14, T__14=15, T__15=16, T__16=17, 
		T__17=18, T__18=19, IDENTIFIER=20, BOOL_OPERATOR=21, WS=22, INTEGER=23, 
		FLOAT=24, STRING=25, BOOL=26, NULL=27;
	public static String[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static String[] modeNames = {
		"DEFAULT_MODE"
	};

	private static String[] makeRuleNames() {
		return new String[] {
			"T__0", "T__1", "T__2", "T__3", "T__4", "T__5", "T__6", "T__7", "T__8", 
			"T__9", "T__10", "T__11", "T__12", "T__13", "T__14", "T__15", "T__16", 
			"T__17", "T__18", "IDENTIFIER", "BOOL_OPERATOR", "WS", "INTEGER", "FLOAT", 
			"STRING", "BOOL", "NULL"
		};
	}
	public static final String[] ruleNames = makeRuleNames();

	private static String[] makeLiteralNames() {
		return new String[] {
			null, "';'", "'='", "'('", "','", "')'", "'!'", "'*'", "'/'", "'%'", 
			"'+'", "'-'", "'=='", "'!='", "'>'", "'<'", "'>='", "'<='", "'{'", "'}'", 
			null, null, null, null, null, null, null, "'null'"
		};
	}
	private static final String[] _LITERAL_NAMES = makeLiteralNames();
	private static String[] makeSymbolicNames() {
		return new String[] {
			null, null, null, null, null, null, null, null, null, null, null, null, 
			null, null, null, null, null, null, null, null, "IDENTIFIER", "BOOL_OPERATOR", 
			"WS", "INTEGER", "FLOAT", "STRING", "BOOL", "NULL"
		};
	}
	private static final String[] _SYMBOLIC_NAMES = makeSymbolicNames();
	public static final Vocabulary VOCABULARY = new VocabularyImpl(_LITERAL_NAMES, _SYMBOLIC_NAMES);

	/**
	 * @deprecated Use {@link #VOCABULARY} instead.
	 */
	@Deprecated
	public static final String[] tokenNames;
	static {
		tokenNames = new String[_SYMBOLIC_NAMES.length];
		for (int i = 0; i < tokenNames.length; i++) {
			tokenNames[i] = VOCABULARY.getLiteralName(i);
			if (tokenNames[i] == null) {
				tokenNames[i] = VOCABULARY.getSymbolicName(i);
			}

			if (tokenNames[i] == null) {
				tokenNames[i] = "<INVALID>";
			}
		}
	}

	@Override
	@Deprecated
	public String[] getTokenNames() {
		return tokenNames;
	}

	@Override

	public Vocabulary getVocabulary() {
		return VOCABULARY;
	}


	public TestTextFileLexer(CharStream input) {
		super(input);
		_interp = new LexerATNSimulator(this,_ATN,_decisionToDFA,_sharedContextCache);
	}

	@Override
	public String getGrammarFileName() { return "TestTextFile.g4"; }

	@Override
	public String[] getRuleNames() { return ruleNames; }

	@Override
	public String getSerializedATN() { return _serializedATN; }

	@Override
	public String[] getChannelNames() { return channelNames; }

	@Override
	public String[] getModeNames() { return modeNames; }

	@Override
	public ATN getATN() { return _ATN; }

	public static final String _serializedATN =
		"\u0004\u0000\u001b\u00a7\u0006\uffff\uffff\u0002\u0000\u0007\u0000\u0002"+
		"\u0001\u0007\u0001\u0002\u0002\u0007\u0002\u0002\u0003\u0007\u0003\u0002"+
		"\u0004\u0007\u0004\u0002\u0005\u0007\u0005\u0002\u0006\u0007\u0006\u0002"+
		"\u0007\u0007\u0007\u0002\b\u0007\b\u0002\t\u0007\t\u0002\n\u0007\n\u0002"+
		"\u000b\u0007\u000b\u0002\f\u0007\f\u0002\r\u0007\r\u0002\u000e\u0007\u000e"+
		"\u0002\u000f\u0007\u000f\u0002\u0010\u0007\u0010\u0002\u0011\u0007\u0011"+
		"\u0002\u0012\u0007\u0012\u0002\u0013\u0007\u0013\u0002\u0014\u0007\u0014"+
		"\u0002\u0015\u0007\u0015\u0002\u0016\u0007\u0016\u0002\u0017\u0007\u0017"+
		"\u0002\u0018\u0007\u0018\u0002\u0019\u0007\u0019\u0002\u001a\u0007\u001a"+
		"\u0001\u0000\u0001\u0000\u0001\u0001\u0001\u0001\u0001\u0002\u0001\u0002"+
		"\u0001\u0003\u0001\u0003\u0001\u0004\u0001\u0004\u0001\u0005\u0001\u0005"+
		"\u0001\u0006\u0001\u0006\u0001\u0007\u0001\u0007\u0001\b\u0001\b\u0001"+
		"\t\u0001\t\u0001\n\u0001\n\u0001\u000b\u0001\u000b\u0001\u000b\u0001\f"+
		"\u0001\f\u0001\f\u0001\r\u0001\r\u0001\u000e\u0001\u000e\u0001\u000f\u0001"+
		"\u000f\u0001\u000f\u0001\u0010\u0001\u0010\u0001\u0010\u0001\u0011\u0001"+
		"\u0011\u0001\u0012\u0001\u0012\u0001\u0013\u0001\u0013\u0005\u0013d\b"+
		"\u0013\n\u0013\f\u0013g\t\u0013\u0001\u0014\u0001\u0014\u0001\u0014\u0001"+
		"\u0014\u0003\u0014m\b\u0014\u0001\u0015\u0004\u0015p\b\u0015\u000b\u0015"+
		"\f\u0015q\u0001\u0015\u0001\u0015\u0001\u0016\u0004\u0016w\b\u0016\u000b"+
		"\u0016\f\u0016x\u0001\u0017\u0004\u0017|\b\u0017\u000b\u0017\f\u0017}"+
		"\u0001\u0017\u0001\u0017\u0004\u0017\u0082\b\u0017\u000b\u0017\f\u0017"+
		"\u0083\u0001\u0018\u0001\u0018\u0005\u0018\u0088\b\u0018\n\u0018\f\u0018"+
		"\u008b\t\u0018\u0001\u0018\u0001\u0018\u0001\u0018\u0005\u0018\u0090\b"+
		"\u0018\n\u0018\f\u0018\u0093\t\u0018\u0001\u0018\u0003\u0018\u0096\b\u0018"+
		"\u0001\u0019\u0001\u0019\u0001\u0019\u0001\u0019\u0001\u0019\u0001\u0019"+
		"\u0001\u0019\u0001\u0019\u0001\u0019\u0003\u0019\u00a1\b\u0019\u0001\u001a"+
		"\u0001\u001a\u0001\u001a\u0001\u001a\u0001\u001a\u0000\u0000\u001b\u0001"+
		"\u0001\u0003\u0002\u0005\u0003\u0007\u0004\t\u0005\u000b\u0006\r\u0007"+
		"\u000f\b\u0011\t\u0013\n\u0015\u000b\u0017\f\u0019\r\u001b\u000e\u001d"+
		"\u000f\u001f\u0010!\u0011#\u0012%\u0013\'\u0014)\u0015+\u0016-\u0017/"+
		"\u00181\u00193\u001a5\u001b\u0001\u0000\u0006\u0002\u0000AZaz\u0004\u0000"+
		"09AZ__az\u0003\u0000\t\n\r\r  \u0001\u000009\u0001\u0000\"\"\u0001\u0000"+
		"\'\'\u00b0\u0000\u0001\u0001\u0000\u0000\u0000\u0000\u0003\u0001\u0000"+
		"\u0000\u0000\u0000\u0005\u0001\u0000\u0000\u0000\u0000\u0007\u0001\u0000"+
		"\u0000\u0000\u0000\t\u0001\u0000\u0000\u0000\u0000\u000b\u0001\u0000\u0000"+
		"\u0000\u0000\r\u0001\u0000\u0000\u0000\u0000\u000f\u0001\u0000\u0000\u0000"+
		"\u0000\u0011\u0001\u0000\u0000\u0000\u0000\u0013\u0001\u0000\u0000\u0000"+
		"\u0000\u0015\u0001\u0000\u0000\u0000\u0000\u0017\u0001\u0000\u0000\u0000"+
		"\u0000\u0019\u0001\u0000\u0000\u0000\u0000\u001b\u0001\u0000\u0000\u0000"+
		"\u0000\u001d\u0001\u0000\u0000\u0000\u0000\u001f\u0001\u0000\u0000\u0000"+
		"\u0000!\u0001\u0000\u0000\u0000\u0000#\u0001\u0000\u0000\u0000\u0000%"+
		"\u0001\u0000\u0000\u0000\u0000\'\u0001\u0000\u0000\u0000\u0000)\u0001"+
		"\u0000\u0000\u0000\u0000+\u0001\u0000\u0000\u0000\u0000-\u0001\u0000\u0000"+
		"\u0000\u0000/\u0001\u0000\u0000\u0000\u00001\u0001\u0000\u0000\u0000\u0000"+
		"3\u0001\u0000\u0000\u0000\u00005\u0001\u0000\u0000\u0000\u00017\u0001"+
		"\u0000\u0000\u0000\u00039\u0001\u0000\u0000\u0000\u0005;\u0001\u0000\u0000"+
		"\u0000\u0007=\u0001\u0000\u0000\u0000\t?\u0001\u0000\u0000\u0000\u000b"+
		"A\u0001\u0000\u0000\u0000\rC\u0001\u0000\u0000\u0000\u000fE\u0001\u0000"+
		"\u0000\u0000\u0011G\u0001\u0000\u0000\u0000\u0013I\u0001\u0000\u0000\u0000"+
		"\u0015K\u0001\u0000\u0000\u0000\u0017M\u0001\u0000\u0000\u0000\u0019P"+
		"\u0001\u0000\u0000\u0000\u001bS\u0001\u0000\u0000\u0000\u001dU\u0001\u0000"+
		"\u0000\u0000\u001fW\u0001\u0000\u0000\u0000!Z\u0001\u0000\u0000\u0000"+
		"#]\u0001\u0000\u0000\u0000%_\u0001\u0000\u0000\u0000\'a\u0001\u0000\u0000"+
		"\u0000)l\u0001\u0000\u0000\u0000+o\u0001\u0000\u0000\u0000-v\u0001\u0000"+
		"\u0000\u0000/{\u0001\u0000\u0000\u00001\u0095\u0001\u0000\u0000\u0000"+
		"3\u00a0\u0001\u0000\u0000\u00005\u00a2\u0001\u0000\u0000\u000078\u0005"+
		";\u0000\u00008\u0002\u0001\u0000\u0000\u00009:\u0005=\u0000\u0000:\u0004"+
		"\u0001\u0000\u0000\u0000;<\u0005(\u0000\u0000<\u0006\u0001\u0000\u0000"+
		"\u0000=>\u0005,\u0000\u0000>\b\u0001\u0000\u0000\u0000?@\u0005)\u0000"+
		"\u0000@\n\u0001\u0000\u0000\u0000AB\u0005!\u0000\u0000B\f\u0001\u0000"+
		"\u0000\u0000CD\u0005*\u0000\u0000D\u000e\u0001\u0000\u0000\u0000EF\u0005"+
		"/\u0000\u0000F\u0010\u0001\u0000\u0000\u0000GH\u0005%\u0000\u0000H\u0012"+
		"\u0001\u0000\u0000\u0000IJ\u0005+\u0000\u0000J\u0014\u0001\u0000\u0000"+
		"\u0000KL\u0005-\u0000\u0000L\u0016\u0001\u0000\u0000\u0000MN\u0005=\u0000"+
		"\u0000NO\u0005=\u0000\u0000O\u0018\u0001\u0000\u0000\u0000PQ\u0005!\u0000"+
		"\u0000QR\u0005=\u0000\u0000R\u001a\u0001\u0000\u0000\u0000ST\u0005>\u0000"+
		"\u0000T\u001c\u0001\u0000\u0000\u0000UV\u0005<\u0000\u0000V\u001e\u0001"+
		"\u0000\u0000\u0000WX\u0005>\u0000\u0000XY\u0005=\u0000\u0000Y \u0001\u0000"+
		"\u0000\u0000Z[\u0005<\u0000\u0000[\\\u0005=\u0000\u0000\\\"\u0001\u0000"+
		"\u0000\u0000]^\u0005{\u0000\u0000^$\u0001\u0000\u0000\u0000_`\u0005}\u0000"+
		"\u0000`&\u0001\u0000\u0000\u0000ae\u0007\u0000\u0000\u0000bd\u0007\u0001"+
		"\u0000\u0000cb\u0001\u0000\u0000\u0000dg\u0001\u0000\u0000\u0000ec\u0001"+
		"\u0000\u0000\u0000ef\u0001\u0000\u0000\u0000f(\u0001\u0000\u0000\u0000"+
		"ge\u0001\u0000\u0000\u0000hi\u0005&\u0000\u0000im\u0005&\u0000\u0000j"+
		"k\u0005|\u0000\u0000km\u0005|\u0000\u0000lh\u0001\u0000\u0000\u0000lj"+
		"\u0001\u0000\u0000\u0000m*\u0001\u0000\u0000\u0000np\u0007\u0002\u0000"+
		"\u0000on\u0001\u0000\u0000\u0000pq\u0001\u0000\u0000\u0000qo\u0001\u0000"+
		"\u0000\u0000qr\u0001\u0000\u0000\u0000rs\u0001\u0000\u0000\u0000st\u0006"+
		"\u0015\u0000\u0000t,\u0001\u0000\u0000\u0000uw\u0007\u0003\u0000\u0000"+
		"vu\u0001\u0000\u0000\u0000wx\u0001\u0000\u0000\u0000xv\u0001\u0000\u0000"+
		"\u0000xy\u0001\u0000\u0000\u0000y.\u0001\u0000\u0000\u0000z|\u0007\u0003"+
		"\u0000\u0000{z\u0001\u0000\u0000\u0000|}\u0001\u0000\u0000\u0000}{\u0001"+
		"\u0000\u0000\u0000}~\u0001\u0000\u0000\u0000~\u007f\u0001\u0000\u0000"+
		"\u0000\u007f\u0081\u0005.\u0000\u0000\u0080\u0082\u0007\u0003\u0000\u0000"+
		"\u0081\u0080\u0001\u0000\u0000\u0000\u0082\u0083\u0001\u0000\u0000\u0000"+
		"\u0083\u0081\u0001\u0000\u0000\u0000\u0083\u0084\u0001\u0000\u0000\u0000"+
		"\u00840\u0001\u0000\u0000\u0000\u0085\u0089\u0005\"\u0000\u0000\u0086"+
		"\u0088\b\u0004\u0000\u0000\u0087\u0086\u0001\u0000\u0000\u0000\u0088\u008b"+
		"\u0001\u0000\u0000\u0000\u0089\u0087\u0001\u0000\u0000\u0000\u0089\u008a"+
		"\u0001\u0000\u0000\u0000\u008a\u008c\u0001\u0000\u0000\u0000\u008b\u0089"+
		"\u0001\u0000\u0000\u0000\u008c\u0096\u0005\"\u0000\u0000\u008d\u0091\u0005"+
		"\'\u0000\u0000\u008e\u0090\b\u0005\u0000\u0000\u008f\u008e\u0001\u0000"+
		"\u0000\u0000\u0090\u0093\u0001\u0000\u0000\u0000\u0091\u008f\u0001\u0000"+
		"\u0000\u0000\u0091\u0092\u0001\u0000\u0000\u0000\u0092\u0094\u0001\u0000"+
		"\u0000\u0000\u0093\u0091\u0001\u0000\u0000\u0000\u0094\u0096\u0005\'\u0000"+
		"\u0000\u0095\u0085\u0001\u0000\u0000\u0000\u0095\u008d\u0001\u0000\u0000"+
		"\u0000\u00962\u0001\u0000\u0000\u0000\u0097\u0098\u0005t\u0000\u0000\u0098"+
		"\u0099\u0005r\u0000\u0000\u0099\u009a\u0005u\u0000\u0000\u009a\u00a1\u0005"+
		"e\u0000\u0000\u009b\u009c\u0005f\u0000\u0000\u009c\u009d\u0005a\u0000"+
		"\u0000\u009d\u009e\u0005l\u0000\u0000\u009e\u009f\u0005s\u0000\u0000\u009f"+
		"\u00a1\u0005e\u0000\u0000\u00a0\u0097\u0001\u0000\u0000\u0000\u00a0\u009b"+
		"\u0001\u0000\u0000\u0000\u00a14\u0001\u0000\u0000\u0000\u00a2\u00a3\u0005"+
		"n\u0000\u0000\u00a3\u00a4\u0005u\u0000\u0000\u00a4\u00a5\u0005l\u0000"+
		"\u0000\u00a5\u00a6\u0005l\u0000\u0000\u00a66\u0001\u0000\u0000\u0000\u000b"+
		"\u0000elqx}\u0083\u0089\u0091\u0095\u00a0\u0001\u0006\u0000\u0000";
	public static final ATN _ATN =
		new ATNDeserializer().deserialize(_serializedATN.toCharArray());
	static {
		_decisionToDFA = new DFA[_ATN.getNumberOfDecisions()];
		for (int i = 0; i < _ATN.getNumberOfDecisions(); i++) {
			_decisionToDFA[i] = new DFA(_ATN.getDecisionState(i), i);
		}
	}
}