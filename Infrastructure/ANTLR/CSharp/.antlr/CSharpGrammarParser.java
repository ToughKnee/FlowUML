// Generated from c:/Users/USUARIO/source/repos/FlowUML/Infrastructure/ANTLR/CSharp/CSharpGrammar.g4 by ANTLR 4.13.1
import org.antlr.v4.runtime.atn.*;
import org.antlr.v4.runtime.dfa.DFA;
import org.antlr.v4.runtime.*;
import org.antlr.v4.runtime.misc.*;
import org.antlr.v4.runtime.tree.*;
import java.util.List;
import java.util.Iterator;
import java.util.ArrayList;

@SuppressWarnings({"all", "warnings", "unchecked", "unused", "cast", "CheckReturnValue"})
public class CSharpGrammarParser extends Parser {
	static { RuntimeMetaData.checkVersion("4.13.1", RuntimeMetaData.VERSION); }

	protected static final DFA[] _decisionToDFA;
	protected static final PredictionContextCache _sharedContextCache =
		new PredictionContextCache();
	public static final int
		T__0=1, T__1=2, T__2=3, T__3=4, T__4=5, T__5=6, T__6=7, T__7=8, T__8=9, 
		T__9=10, T__10=11, T__11=12, T__12=13, T__13=14, T__14=15, T__15=16, T__16=17, 
		T__17=18, T__18=19, T__19=20, T__20=21, T__21=22, T__22=23, T__23=24, 
		T__24=25, T__25=26, T__26=27, T__27=28, T__28=29, T__29=30, T__30=31, 
		T__31=32, T__32=33, T__33=34, T__34=35, T__35=36, T__36=37, WS=38, IDENTIFIER=39, 
		WordSequence=40, Word=41;
	public static final int
		RULE_typeName = 0, RULE_accessModifier = 1, RULE_classDeclaration = 2, 
		RULE_classBodyContent = 3, RULE_classHeritage = 4, RULE_propertyDeclaration = 5, 
		RULE_modifiers = 6, RULE_attributeIdentifier = 7, RULE_attributes = 8, 
		RULE_methodDeclaration = 9, RULE_methodBodyContent = 10, RULE_parameterList = 11, 
		RULE_parameter = 12;
	private static String[] makeRuleNames() {
		return new String[] {
			"typeName", "accessModifier", "classDeclaration", "classBodyContent", 
			"classHeritage", "propertyDeclaration", "modifiers", "attributeIdentifier", 
			"attributes", "methodDeclaration", "methodBodyContent", "parameterList", 
			"parameter"
		};
	}
	public static final String[] ruleNames = makeRuleNames();

	private static String[] makeLiteralNames() {
		return new String[] {
			null, "'string'", "'char'", "'bool'", "'int'", "'uint'", "'float'", "'long'", 
			"'ulong'", "'short'", "'ushort'", "'byte'", "'decimal'", "'var'", "'object'", 
			"'dynamic'", "'<'", "','", "'>'", "'public'", "'protected'", "'private'", 
			"'internal'", "'class'", "'{'", "'}'", "':'", "'('", "'static'", "'virtual'", 
			"'override'", "'abstract'", "'sealed'", "'readonly'", "'async'", "'['", 
			"')'", "']'"
		};
	}
	private static final String[] _LITERAL_NAMES = makeLiteralNames();
	private static String[] makeSymbolicNames() {
		return new String[] {
			null, null, null, null, null, null, null, null, null, null, null, null, 
			null, null, null, null, null, null, null, null, null, null, null, null, 
			null, null, null, null, null, null, null, null, null, null, null, null, 
			null, null, "WS", "IDENTIFIER", "WordSequence", "Word"
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

	@Override
	public String getGrammarFileName() { return "CSharpGrammar.g4"; }

	@Override
	public String[] getRuleNames() { return ruleNames; }

	@Override
	public String getSerializedATN() { return _serializedATN; }

	@Override
	public ATN getATN() { return _ATN; }

	public CSharpGrammarParser(TokenStream input) {
		super(input);
		_interp = new ParserATNSimulator(this,_ATN,_decisionToDFA,_sharedContextCache);
	}

	@SuppressWarnings("CheckReturnValue")
	public static class TypeNameContext extends ParserRuleContext {
		public List<TerminalNode> IDENTIFIER() { return getTokens(CSharpGrammarParser.IDENTIFIER); }
		public TerminalNode IDENTIFIER(int i) {
			return getToken(CSharpGrammarParser.IDENTIFIER, i);
		}
		public TypeNameContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_typeName; }
	}

	public final TypeNameContext typeName() throws RecognitionException {
		TypeNameContext _localctx = new TypeNameContext(_ctx, getState());
		enterRule(_localctx, 0, RULE_typeName);
		int _la;
		try {
			setState(54);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case T__0:
				enterOuterAlt(_localctx, 1);
				{
				setState(26);
				match(T__0);
				}
				break;
			case T__1:
				enterOuterAlt(_localctx, 2);
				{
				setState(27);
				match(T__1);
				}
				break;
			case T__2:
				enterOuterAlt(_localctx, 3);
				{
				setState(28);
				match(T__2);
				}
				break;
			case T__3:
				enterOuterAlt(_localctx, 4);
				{
				setState(29);
				match(T__3);
				}
				break;
			case T__4:
				enterOuterAlt(_localctx, 5);
				{
				setState(30);
				match(T__4);
				}
				break;
			case T__5:
				enterOuterAlt(_localctx, 6);
				{
				setState(31);
				match(T__5);
				}
				break;
			case T__6:
				enterOuterAlt(_localctx, 7);
				{
				setState(32);
				match(T__6);
				}
				break;
			case T__7:
				enterOuterAlt(_localctx, 8);
				{
				setState(33);
				match(T__7);
				}
				break;
			case T__8:
				enterOuterAlt(_localctx, 9);
				{
				setState(34);
				match(T__8);
				}
				break;
			case T__9:
				enterOuterAlt(_localctx, 10);
				{
				setState(35);
				match(T__9);
				}
				break;
			case T__10:
				enterOuterAlt(_localctx, 11);
				{
				setState(36);
				match(T__10);
				}
				break;
			case T__11:
				enterOuterAlt(_localctx, 12);
				{
				setState(37);
				match(T__11);
				}
				break;
			case T__12:
				enterOuterAlt(_localctx, 13);
				{
				setState(38);
				match(T__12);
				}
				break;
			case T__13:
				enterOuterAlt(_localctx, 14);
				{
				setState(39);
				match(T__13);
				}
				break;
			case T__14:
				enterOuterAlt(_localctx, 15);
				{
				setState(40);
				match(T__14);
				}
				break;
			case IDENTIFIER:
				enterOuterAlt(_localctx, 16);
				{
				setState(41);
				match(IDENTIFIER);
				setState(52);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==T__15) {
					{
					setState(42);
					match(T__15);
					setState(43);
					match(IDENTIFIER);
					setState(48);
					_errHandler.sync(this);
					_la = _input.LA(1);
					while (_la==T__16) {
						{
						{
						setState(44);
						match(T__16);
						setState(45);
						match(IDENTIFIER);
						}
						}
						setState(50);
						_errHandler.sync(this);
						_la = _input.LA(1);
					}
					setState(51);
					match(T__17);
					}
				}

				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class AccessModifierContext extends ParserRuleContext {
		public AccessModifierContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_accessModifier; }
	}

	public final AccessModifierContext accessModifier() throws RecognitionException {
		AccessModifierContext _localctx = new AccessModifierContext(_ctx, getState());
		enterRule(_localctx, 2, RULE_accessModifier);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(56);
			_la = _input.LA(1);
			if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & 7864320L) != 0)) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class ClassDeclarationContext extends ParserRuleContext {
		public TerminalNode IDENTIFIER() { return getToken(CSharpGrammarParser.IDENTIFIER, 0); }
		public AttributesContext attributes() {
			return getRuleContext(AttributesContext.class,0);
		}
		public AccessModifierContext accessModifier() {
			return getRuleContext(AccessModifierContext.class,0);
		}
		public ClassHeritageContext classHeritage() {
			return getRuleContext(ClassHeritageContext.class,0);
		}
		public ClassBodyContentContext classBodyContent() {
			return getRuleContext(ClassBodyContentContext.class,0);
		}
		public ClassDeclarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_classDeclaration; }
	}

	public final ClassDeclarationContext classDeclaration() throws RecognitionException {
		ClassDeclarationContext _localctx = new ClassDeclarationContext(_ctx, getState());
		enterRule(_localctx, 4, RULE_classDeclaration);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(59);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==T__34) {
				{
				setState(58);
				attributes();
				}
			}

			setState(62);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & 7864320L) != 0)) {
				{
				setState(61);
				accessModifier();
				}
			}

			setState(64);
			match(T__22);
			setState(65);
			match(IDENTIFIER);
			setState(67);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==T__25) {
				{
				setState(66);
				classHeritage();
				}
			}

			setState(70);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==T__23) {
				{
				setState(69);
				classBodyContent();
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class ClassBodyContentContext extends ParserRuleContext {
		public List<TerminalNode> WordSequence() { return getTokens(CSharpGrammarParser.WordSequence); }
		public TerminalNode WordSequence(int i) {
			return getToken(CSharpGrammarParser.WordSequence, i);
		}
		public ClassBodyContentContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_classBodyContent; }
	}

	public final ClassBodyContentContext classBodyContent() throws RecognitionException {
		ClassBodyContentContext _localctx = new ClassBodyContentContext(_ctx, getState());
		enterRule(_localctx, 6, RULE_classBodyContent);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(72);
			match(T__23);
			setState(76);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==WordSequence) {
				{
				{
				setState(73);
				match(WordSequence);
				}
				}
				setState(78);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(79);
			match(T__24);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class ClassHeritageContext extends ParserRuleContext {
		public List<TypeNameContext> typeName() {
			return getRuleContexts(TypeNameContext.class);
		}
		public TypeNameContext typeName(int i) {
			return getRuleContext(TypeNameContext.class,i);
		}
		public ClassHeritageContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_classHeritage; }
	}

	public final ClassHeritageContext classHeritage() throws RecognitionException {
		ClassHeritageContext _localctx = new ClassHeritageContext(_ctx, getState());
		enterRule(_localctx, 8, RULE_classHeritage);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(81);
			match(T__25);
			setState(82);
			typeName();
			setState(87);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==T__16) {
				{
				{
				setState(83);
				match(T__16);
				setState(84);
				typeName();
				}
				}
				setState(89);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class PropertyDeclarationContext extends ParserRuleContext {
		public TypeNameContext typeName() {
			return getRuleContext(TypeNameContext.class,0);
		}
		public TerminalNode IDENTIFIER() { return getToken(CSharpGrammarParser.IDENTIFIER, 0); }
		public AttributesContext attributes() {
			return getRuleContext(AttributesContext.class,0);
		}
		public AccessModifierContext accessModifier() {
			return getRuleContext(AccessModifierContext.class,0);
		}
		public ModifiersContext modifiers() {
			return getRuleContext(ModifiersContext.class,0);
		}
		public PropertyDeclarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_propertyDeclaration; }
	}

	public final PropertyDeclarationContext propertyDeclaration() throws RecognitionException {
		PropertyDeclarationContext _localctx = new PropertyDeclarationContext(_ctx, getState());
		enterRule(_localctx, 10, RULE_propertyDeclaration);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(91);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==T__34) {
				{
				setState(90);
				attributes();
				}
			}

			setState(94);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & 7864320L) != 0)) {
				{
				setState(93);
				accessModifier();
				}
			}

			setState(97);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & 34091302912L) != 0)) {
				{
				setState(96);
				modifiers();
				}
			}

			setState(99);
			typeName();
			setState(100);
			match(IDENTIFIER);
			setState(101);
			match(T__26);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class ModifiersContext extends ParserRuleContext {
		public ModifiersContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_modifiers; }
	}

	public final ModifiersContext modifiers() throws RecognitionException {
		ModifiersContext _localctx = new ModifiersContext(_ctx, getState());
		enterRule(_localctx, 12, RULE_modifiers);
		int _la;
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(104); 
			_errHandler.sync(this);
			_alt = 1;
			do {
				switch (_alt) {
				case 1:
					{
					{
					setState(103);
					_la = _input.LA(1);
					if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & 34091302912L) != 0)) ) {
					_errHandler.recoverInline(this);
					}
					else {
						if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
						_errHandler.reportMatch(this);
						consume();
					}
					}
					}
					break;
				default:
					throw new NoViableAltException(this);
				}
				setState(106); 
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,12,_ctx);
			} while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER );
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class AttributeIdentifierContext extends ParserRuleContext {
		public TerminalNode IDENTIFIER() { return getToken(CSharpGrammarParser.IDENTIFIER, 0); }
		public AttributeIdentifierContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_attributeIdentifier; }
	}

	public final AttributeIdentifierContext attributeIdentifier() throws RecognitionException {
		AttributeIdentifierContext _localctx = new AttributeIdentifierContext(_ctx, getState());
		enterRule(_localctx, 14, RULE_attributeIdentifier);
		int _la;
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(108);
			match(T__34);
			setState(109);
			match(IDENTIFIER);
			setState(118);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==T__26) {
				{
				setState(110);
				match(T__26);
				setState(114);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,13,_ctx);
				while ( _alt!=1 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
					if ( _alt==1+1 ) {
						{
						{
						setState(111);
						matchWildcard();
						}
						} 
					}
					setState(116);
					_errHandler.sync(this);
					_alt = getInterpreter().adaptivePredict(_input,13,_ctx);
				}
				setState(117);
				match(T__35);
				}
			}

			setState(120);
			match(T__36);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class AttributesContext extends ParserRuleContext {
		public List<AttributeIdentifierContext> attributeIdentifier() {
			return getRuleContexts(AttributeIdentifierContext.class);
		}
		public AttributeIdentifierContext attributeIdentifier(int i) {
			return getRuleContext(AttributeIdentifierContext.class,i);
		}
		public AttributesContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_attributes; }
	}

	public final AttributesContext attributes() throws RecognitionException {
		AttributesContext _localctx = new AttributesContext(_ctx, getState());
		enterRule(_localctx, 16, RULE_attributes);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(122);
			attributeIdentifier();
			setState(127);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==T__16) {
				{
				{
				setState(123);
				match(T__16);
				setState(124);
				attributeIdentifier();
				}
				}
				setState(129);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class MethodDeclarationContext extends ParserRuleContext {
		public TypeNameContext typeName() {
			return getRuleContext(TypeNameContext.class,0);
		}
		public TerminalNode IDENTIFIER() { return getToken(CSharpGrammarParser.IDENTIFIER, 0); }
		public AccessModifierContext accessModifier() {
			return getRuleContext(AccessModifierContext.class,0);
		}
		public List<ModifiersContext> modifiers() {
			return getRuleContexts(ModifiersContext.class);
		}
		public ModifiersContext modifiers(int i) {
			return getRuleContext(ModifiersContext.class,i);
		}
		public ParameterListContext parameterList() {
			return getRuleContext(ParameterListContext.class,0);
		}
		public MethodDeclarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_methodDeclaration; }
	}

	public final MethodDeclarationContext methodDeclaration() throws RecognitionException {
		MethodDeclarationContext _localctx = new MethodDeclarationContext(_ctx, getState());
		enterRule(_localctx, 18, RULE_methodDeclaration);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(131);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & 7864320L) != 0)) {
				{
				setState(130);
				accessModifier();
				}
			}

			setState(136);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while ((((_la) & ~0x3f) == 0 && ((1L << _la) & 34091302912L) != 0)) {
				{
				{
				setState(133);
				modifiers();
				}
				}
				setState(138);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(139);
			typeName();
			setState(140);
			match(IDENTIFIER);
			setState(141);
			match(T__26);
			setState(143);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & 549755879422L) != 0)) {
				{
				setState(142);
				parameterList();
				}
			}

			setState(145);
			match(T__35);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class MethodBodyContentContext extends ParserRuleContext {
		public MethodBodyContentContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_methodBodyContent; }
	}

	public final MethodBodyContentContext methodBodyContent() throws RecognitionException {
		MethodBodyContentContext _localctx = new MethodBodyContentContext(_ctx, getState());
		enterRule(_localctx, 20, RULE_methodBodyContent);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(147);
			match(T__23);
			setState(148);
			match(T__24);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class ParameterListContext extends ParserRuleContext {
		public List<ParameterContext> parameter() {
			return getRuleContexts(ParameterContext.class);
		}
		public ParameterContext parameter(int i) {
			return getRuleContext(ParameterContext.class,i);
		}
		public ParameterListContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_parameterList; }
	}

	public final ParameterListContext parameterList() throws RecognitionException {
		ParameterListContext _localctx = new ParameterListContext(_ctx, getState());
		enterRule(_localctx, 22, RULE_parameterList);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(150);
			parameter();
			setState(155);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==T__16) {
				{
				{
				setState(151);
				match(T__16);
				setState(152);
				parameter();
				}
				}
				setState(157);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class ParameterContext extends ParserRuleContext {
		public TypeNameContext typeName() {
			return getRuleContext(TypeNameContext.class,0);
		}
		public TerminalNode IDENTIFIER() { return getToken(CSharpGrammarParser.IDENTIFIER, 0); }
		public ParameterContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_parameter; }
	}

	public final ParameterContext parameter() throws RecognitionException {
		ParameterContext _localctx = new ParameterContext(_ctx, getState());
		enterRule(_localctx, 24, RULE_parameter);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(158);
			typeName();
			setState(159);
			match(IDENTIFIER);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static final String _serializedATN =
		"\u0004\u0001)\u00a2\u0002\u0000\u0007\u0000\u0002\u0001\u0007\u0001\u0002"+
		"\u0002\u0007\u0002\u0002\u0003\u0007\u0003\u0002\u0004\u0007\u0004\u0002"+
		"\u0005\u0007\u0005\u0002\u0006\u0007\u0006\u0002\u0007\u0007\u0007\u0002"+
		"\b\u0007\b\u0002\t\u0007\t\u0002\n\u0007\n\u0002\u000b\u0007\u000b\u0002"+
		"\f\u0007\f\u0001\u0000\u0001\u0000\u0001\u0000\u0001\u0000\u0001\u0000"+
		"\u0001\u0000\u0001\u0000\u0001\u0000\u0001\u0000\u0001\u0000\u0001\u0000"+
		"\u0001\u0000\u0001\u0000\u0001\u0000\u0001\u0000\u0001\u0000\u0001\u0000"+
		"\u0001\u0000\u0001\u0000\u0001\u0000\u0005\u0000/\b\u0000\n\u0000\f\u0000"+
		"2\t\u0000\u0001\u0000\u0003\u00005\b\u0000\u0003\u00007\b\u0000\u0001"+
		"\u0001\u0001\u0001\u0001\u0002\u0003\u0002<\b\u0002\u0001\u0002\u0003"+
		"\u0002?\b\u0002\u0001\u0002\u0001\u0002\u0001\u0002\u0003\u0002D\b\u0002"+
		"\u0001\u0002\u0003\u0002G\b\u0002\u0001\u0003\u0001\u0003\u0005\u0003"+
		"K\b\u0003\n\u0003\f\u0003N\t\u0003\u0001\u0003\u0001\u0003\u0001\u0004"+
		"\u0001\u0004\u0001\u0004\u0001\u0004\u0005\u0004V\b\u0004\n\u0004\f\u0004"+
		"Y\t\u0004\u0001\u0005\u0003\u0005\\\b\u0005\u0001\u0005\u0003\u0005_\b"+
		"\u0005\u0001\u0005\u0003\u0005b\b\u0005\u0001\u0005\u0001\u0005\u0001"+
		"\u0005\u0001\u0005\u0001\u0006\u0004\u0006i\b\u0006\u000b\u0006\f\u0006"+
		"j\u0001\u0007\u0001\u0007\u0001\u0007\u0001\u0007\u0005\u0007q\b\u0007"+
		"\n\u0007\f\u0007t\t\u0007\u0001\u0007\u0003\u0007w\b\u0007\u0001\u0007"+
		"\u0001\u0007\u0001\b\u0001\b\u0001\b\u0005\b~\b\b\n\b\f\b\u0081\t\b\u0001"+
		"\t\u0003\t\u0084\b\t\u0001\t\u0005\t\u0087\b\t\n\t\f\t\u008a\t\t\u0001"+
		"\t\u0001\t\u0001\t\u0001\t\u0003\t\u0090\b\t\u0001\t\u0001\t\u0001\n\u0001"+
		"\n\u0001\n\u0001\u000b\u0001\u000b\u0001\u000b\u0005\u000b\u009a\b\u000b"+
		"\n\u000b\f\u000b\u009d\t\u000b\u0001\f\u0001\f\u0001\f\u0001\f\u0001r"+
		"\u0000\r\u0000\u0002\u0004\u0006\b\n\f\u000e\u0010\u0012\u0014\u0016\u0018"+
		"\u0000\u0002\u0001\u0000\u0013\u0016\u0001\u0000\u001c\"\u00b6\u00006"+
		"\u0001\u0000\u0000\u0000\u00028\u0001\u0000\u0000\u0000\u0004;\u0001\u0000"+
		"\u0000\u0000\u0006H\u0001\u0000\u0000\u0000\bQ\u0001\u0000\u0000\u0000"+
		"\n[\u0001\u0000\u0000\u0000\fh\u0001\u0000\u0000\u0000\u000el\u0001\u0000"+
		"\u0000\u0000\u0010z\u0001\u0000\u0000\u0000\u0012\u0083\u0001\u0000\u0000"+
		"\u0000\u0014\u0093\u0001\u0000\u0000\u0000\u0016\u0096\u0001\u0000\u0000"+
		"\u0000\u0018\u009e\u0001\u0000\u0000\u0000\u001a7\u0005\u0001\u0000\u0000"+
		"\u001b7\u0005\u0002\u0000\u0000\u001c7\u0005\u0003\u0000\u0000\u001d7"+
		"\u0005\u0004\u0000\u0000\u001e7\u0005\u0005\u0000\u0000\u001f7\u0005\u0006"+
		"\u0000\u0000 7\u0005\u0007\u0000\u0000!7\u0005\b\u0000\u0000\"7\u0005"+
		"\t\u0000\u0000#7\u0005\n\u0000\u0000$7\u0005\u000b\u0000\u0000%7\u0005"+
		"\f\u0000\u0000&7\u0005\r\u0000\u0000\'7\u0005\u000e\u0000\u0000(7\u0005"+
		"\u000f\u0000\u0000)4\u0005\'\u0000\u0000*+\u0005\u0010\u0000\u0000+0\u0005"+
		"\'\u0000\u0000,-\u0005\u0011\u0000\u0000-/\u0005\'\u0000\u0000.,\u0001"+
		"\u0000\u0000\u0000/2\u0001\u0000\u0000\u00000.\u0001\u0000\u0000\u0000"+
		"01\u0001\u0000\u0000\u000013\u0001\u0000\u0000\u000020\u0001\u0000\u0000"+
		"\u000035\u0005\u0012\u0000\u00004*\u0001\u0000\u0000\u000045\u0001\u0000"+
		"\u0000\u000057\u0001\u0000\u0000\u00006\u001a\u0001\u0000\u0000\u0000"+
		"6\u001b\u0001\u0000\u0000\u00006\u001c\u0001\u0000\u0000\u00006\u001d"+
		"\u0001\u0000\u0000\u00006\u001e\u0001\u0000\u0000\u00006\u001f\u0001\u0000"+
		"\u0000\u00006 \u0001\u0000\u0000\u00006!\u0001\u0000\u0000\u00006\"\u0001"+
		"\u0000\u0000\u00006#\u0001\u0000\u0000\u00006$\u0001\u0000\u0000\u0000"+
		"6%\u0001\u0000\u0000\u00006&\u0001\u0000\u0000\u00006\'\u0001\u0000\u0000"+
		"\u00006(\u0001\u0000\u0000\u00006)\u0001\u0000\u0000\u00007\u0001\u0001"+
		"\u0000\u0000\u000089\u0007\u0000\u0000\u00009\u0003\u0001\u0000\u0000"+
		"\u0000:<\u0003\u0010\b\u0000;:\u0001\u0000\u0000\u0000;<\u0001\u0000\u0000"+
		"\u0000<>\u0001\u0000\u0000\u0000=?\u0003\u0002\u0001\u0000>=\u0001\u0000"+
		"\u0000\u0000>?\u0001\u0000\u0000\u0000?@\u0001\u0000\u0000\u0000@A\u0005"+
		"\u0017\u0000\u0000AC\u0005\'\u0000\u0000BD\u0003\b\u0004\u0000CB\u0001"+
		"\u0000\u0000\u0000CD\u0001\u0000\u0000\u0000DF\u0001\u0000\u0000\u0000"+
		"EG\u0003\u0006\u0003\u0000FE\u0001\u0000\u0000\u0000FG\u0001\u0000\u0000"+
		"\u0000G\u0005\u0001\u0000\u0000\u0000HL\u0005\u0018\u0000\u0000IK\u0005"+
		"(\u0000\u0000JI\u0001\u0000\u0000\u0000KN\u0001\u0000\u0000\u0000LJ\u0001"+
		"\u0000\u0000\u0000LM\u0001\u0000\u0000\u0000MO\u0001\u0000\u0000\u0000"+
		"NL\u0001\u0000\u0000\u0000OP\u0005\u0019\u0000\u0000P\u0007\u0001\u0000"+
		"\u0000\u0000QR\u0005\u001a\u0000\u0000RW\u0003\u0000\u0000\u0000ST\u0005"+
		"\u0011\u0000\u0000TV\u0003\u0000\u0000\u0000US\u0001\u0000\u0000\u0000"+
		"VY\u0001\u0000\u0000\u0000WU\u0001\u0000\u0000\u0000WX\u0001\u0000\u0000"+
		"\u0000X\t\u0001\u0000\u0000\u0000YW\u0001\u0000\u0000\u0000Z\\\u0003\u0010"+
		"\b\u0000[Z\u0001\u0000\u0000\u0000[\\\u0001\u0000\u0000\u0000\\^\u0001"+
		"\u0000\u0000\u0000]_\u0003\u0002\u0001\u0000^]\u0001\u0000\u0000\u0000"+
		"^_\u0001\u0000\u0000\u0000_a\u0001\u0000\u0000\u0000`b\u0003\f\u0006\u0000"+
		"a`\u0001\u0000\u0000\u0000ab\u0001\u0000\u0000\u0000bc\u0001\u0000\u0000"+
		"\u0000cd\u0003\u0000\u0000\u0000de\u0005\'\u0000\u0000ef\u0005\u001b\u0000"+
		"\u0000f\u000b\u0001\u0000\u0000\u0000gi\u0007\u0001\u0000\u0000hg\u0001"+
		"\u0000\u0000\u0000ij\u0001\u0000\u0000\u0000jh\u0001\u0000\u0000\u0000"+
		"jk\u0001\u0000\u0000\u0000k\r\u0001\u0000\u0000\u0000lm\u0005#\u0000\u0000"+
		"mv\u0005\'\u0000\u0000nr\u0005\u001b\u0000\u0000oq\t\u0000\u0000\u0000"+
		"po\u0001\u0000\u0000\u0000qt\u0001\u0000\u0000\u0000rs\u0001\u0000\u0000"+
		"\u0000rp\u0001\u0000\u0000\u0000su\u0001\u0000\u0000\u0000tr\u0001\u0000"+
		"\u0000\u0000uw\u0005$\u0000\u0000vn\u0001\u0000\u0000\u0000vw\u0001\u0000"+
		"\u0000\u0000wx\u0001\u0000\u0000\u0000xy\u0005%\u0000\u0000y\u000f\u0001"+
		"\u0000\u0000\u0000z\u007f\u0003\u000e\u0007\u0000{|\u0005\u0011\u0000"+
		"\u0000|~\u0003\u000e\u0007\u0000}{\u0001\u0000\u0000\u0000~\u0081\u0001"+
		"\u0000\u0000\u0000\u007f}\u0001\u0000\u0000\u0000\u007f\u0080\u0001\u0000"+
		"\u0000\u0000\u0080\u0011\u0001\u0000\u0000\u0000\u0081\u007f\u0001\u0000"+
		"\u0000\u0000\u0082\u0084\u0003\u0002\u0001\u0000\u0083\u0082\u0001\u0000"+
		"\u0000\u0000\u0083\u0084\u0001\u0000\u0000\u0000\u0084\u0088\u0001\u0000"+
		"\u0000\u0000\u0085\u0087\u0003\f\u0006\u0000\u0086\u0085\u0001\u0000\u0000"+
		"\u0000\u0087\u008a\u0001\u0000\u0000\u0000\u0088\u0086\u0001\u0000\u0000"+
		"\u0000\u0088\u0089\u0001\u0000\u0000\u0000\u0089\u008b\u0001\u0000\u0000"+
		"\u0000\u008a\u0088\u0001\u0000\u0000\u0000\u008b\u008c\u0003\u0000\u0000"+
		"\u0000\u008c\u008d\u0005\'\u0000\u0000\u008d\u008f\u0005\u001b\u0000\u0000"+
		"\u008e\u0090\u0003\u0016\u000b\u0000\u008f\u008e\u0001\u0000\u0000\u0000"+
		"\u008f\u0090\u0001\u0000\u0000\u0000\u0090\u0091\u0001\u0000\u0000\u0000"+
		"\u0091\u0092\u0005$\u0000\u0000\u0092\u0013\u0001\u0000\u0000\u0000\u0093"+
		"\u0094\u0005\u0018\u0000\u0000\u0094\u0095\u0005\u0019\u0000\u0000\u0095"+
		"\u0015\u0001\u0000\u0000\u0000\u0096\u009b\u0003\u0018\f\u0000\u0097\u0098"+
		"\u0005\u0011\u0000\u0000\u0098\u009a\u0003\u0018\f\u0000\u0099\u0097\u0001"+
		"\u0000\u0000\u0000\u009a\u009d\u0001\u0000\u0000\u0000\u009b\u0099\u0001"+
		"\u0000\u0000\u0000\u009b\u009c\u0001\u0000\u0000\u0000\u009c\u0017\u0001"+
		"\u0000\u0000\u0000\u009d\u009b\u0001\u0000\u0000\u0000\u009e\u009f\u0003"+
		"\u0000\u0000\u0000\u009f\u00a0\u0005\'\u0000\u0000\u00a0\u0019\u0001\u0000"+
		"\u0000\u0000\u0014046;>CFLW[^ajrv\u007f\u0083\u0088\u008f\u009b";
	public static final ATN _ATN =
		new ATNDeserializer().deserialize(_serializedATN.toCharArray());
	static {
		_decisionToDFA = new DFA[_ATN.getNumberOfDecisions()];
		for (int i = 0; i < _ATN.getNumberOfDecisions(); i++) {
			_decisionToDFA[i] = new DFA(_ATN.getDecisionState(i), i);
		}
	}
}