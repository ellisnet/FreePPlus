// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic;

//was previously: namespace SixLabors.Fonts.Tables.AdvancedTypographic;

/// <summary>
///     Provides enumeration for the different font features.
///     <see href="https://docs.microsoft.com/en-us/typography/opentype/spec/featuretags" />
/// </summary>
public enum FeatureTags : uint
{
    /// <summary>
    ///     Access All Alternates. Shortcode: aalt.
    ///     This feature makes all variations of a selected character accessible. This serves several purposes: An application
    ///     may not support the feature by which the desired glyph would normally be accessed;
    ///     the user may need a glyph outside the context supported by the normal substitution, or the user may not know what
    ///     feature produces the desired glyph.
    ///     Since many-to-one substitutions are not covered, ligatures would not appear in this table unless they were variant
    ///     forms of another ligature.
    /// </summary>
    AccessAllAlternates = 0x61616C74U,

    /// <summary>
    ///     Above-base Forms. Shortcode: abvf.
    ///     Substitutes the above-base form of a vowel.
    /// </summary>
    AboveBaseForms = 0x61627666U,

    /// <summary>
    ///     Above-base Mark Positioning. Shortcode: abvm.
    ///     Positions marks above base glyphs.
    /// </summary>
    AboveBaseMarkPositioning = 0x6162766DU,

    /// <summary>
    ///     Above-base Substitutions. Shortcode: abvs.
    ///     Substitutes a ligature for a base glyph and mark that’s above it.
    /// </summary>
    AboveBaseSubstitutions = 0x61627673U,

    /// <summary>
    ///     Alternative Fractions. Shortccde: afrc.
    ///     Replaces figures separated by a slash with an alternative form.
    /// </summary>
    AlternativeFractions = 0x61667263U,

    /// <summary>
    ///     Akhand. Shortcode: akhn.
    ///     Preferentially substitutes a sequence of characters with a ligature. This substitution is done irrespective of any
    ///     characters that may precede or follow the sequence.
    /// </summary>
    Akhand = 0x616B686EU,

    /// <summary>
    ///     Below-base Forms. Shortcode: blwf.
    ///     Substitutes the below-base form of a consonant in conjuncts.
    /// </summary>
    BelowBaseForms = 0x626C7766U,

    /// <summary>
    ///     Below-base Mark Positioning. Shortcode: blwm.
    ///     Positions marks below base glyphs.
    /// </summary>
    BelowBaseMarkPositioning = 0x626C776DU,

    /// <summary>
    ///     Below-base Substitutions. Shortcode: blws.
    ///     Produces ligatures that comprise of base glyph and below-base forms.
    /// </summary>
    BelowBaseSubstitutions = 0x626C7773U,

    /// <summary>
    ///     Contextual Alternates. Shortcode: calt.
    ///     In specified situations, replaces default glyphs with alternate forms which provide better joining behavior.
    ///     Used in script typefaces which are designed to have some or all of their glyphs join.
    /// </summary>
    ContextualAlternates = 0x63616C74U,

    /// <summary>
    ///     Case-Sensitive Forms. Shortcode: case.
    ///     Shifts various punctuation marks up to a position that works better with all-capital sequences or sets of lining
    ///     figures;
    ///     also changes oldstyle figures to lining figures. By default, glyphs in a text face are designed to work with
    ///     lowercase characters.
    ///     Some characters should be shifted vertically to fit the higher visual center of all-capital or lining text.
    ///     Also, lining figures are the same height (or close to it) as capitals, and fit much better with all-capital text.
    /// </summary>
    CaseSensitiveForms = 0x63617365U,

    /// <summary>
    ///     Glyph Composition/Decomposition. Shortcode: ccmp.
    ///     To minimize the number of glyph alternates, it is sometimes desirable to decompose the default glyph for a
    ///     character into two or more glyphs.
    ///     Additionally, it may be preferable to compose default glyphs for two or more characters into a single glyph for
    ///     better glyph processing.
    ///     This feature permits such composition/decomposition. The feature should be processed as the first feature
    ///     processed, and should be processed only when it is called.
    /// </summary>
    GlyphCompositionDecomposition = 0x63636D70U,

    /// <summary>
    ///     Conjunct Form After Ro. Shortcode: cfar.
    ///     Substitutes alternate below-base or post-base forms in Khmer script when occurring after conjoined Ro (“Coeng Ra”).
    /// </summary>
    ConjunctFormAfterRo = 0x63666172U,

    /// <summary>
    ///     Conjunct Forms. Shortcode: cjct.
    ///     Produces conjunct forms of consonants in Indic scripts. This is similar to the Akhands feature, but is applied at a
    ///     different sequential point in the process of shaping an Indic syllable.
    /// </summary>
    ConjunctForms = 0x636A6374U,

    /// <summary>
    ///     Contextual Ligatures. Shortcode: clig.
    ///     Replaces a sequence of glyphs with a single glyph which is preferred for typographic purposes. Unlike other
    ///     ligature features, 'clig' specifies the context in which the ligature is recommended.
    ///     This capability is important in some script designs and for swash ligatures.
    /// </summary>
    ContextualLigatures = 0x636C6967U,

    /// <summary>
    ///     Centered CJK Punctuation. Shortcode: cpct.
    ///     Centers specific punctuation marks for those fonts that do not include centered and non-centered forms.
    /// </summary>
    CenteredCjkPunctuation = 0x63706374U,

    /// <summary>
    ///     Capital Spacing. Shortcode: cpsp.
    ///     Globally adjusts inter-glyph spacing for all-capital text. Most typefaces contain capitals and lowercase
    ///     characters, and the capitals are positioned to work with the lowercase.
    ///     When capitals are used for words, they need more space between them for legibility and esthetics.
    ///     This feature would not apply to monospaced designs. Of course the user may want to override this behavior in order
    ///     to do more pronounced letterspacing for esthetic reasons.
    /// </summary>
    CapitalSpacing = 0x63707370U,

    /// <summary>
    ///     Contextual Swash. Shortcode: cswh.
    ///     This feature replaces default character glyphs with corresponding swash glyphs in a specified context. Note that
    ///     there may be more than one swash alternate for a given character.
    /// </summary>
    ContextualSwash = 0x63737768U,

    /// <summary>
    ///     Cursive Positioning. Shortcode: curs.
    ///     In cursive scripts like Arabic, this feature cursively positions adjacent glyphs.
    /// </summary>
    CursivePositioning = 0x63757273U,

    /// <summary>
    ///     Petite Capitals From Capitals. Shortcode: c2pc.
    ///     This feature turns capital characters into petite capitals. It is generally used for words which would otherwise be
    ///     set in all caps, such as acronyms,
    ///     but which are desired in petite-cap form to avoid disrupting the flow of text. See the 'pcap' feature description
    ///     for notes on the relationship of caps,
    ///     smallcaps and petite caps.
    /// </summary>
    PetiteCapitalsFromCapitals = 0x63327063U,

    /// <summary>
    ///     Small Capitals From Capitals. Shortcode: c2sc.
    ///     This feature turns capital characters into small capitals. It is generally used for words which would otherwise be
    ///     set in all caps,
    ///     such as acronyms, but which are desired in small-cap form to avoid disrupting the flow of text.
    /// </summary>
    SmallCapitalsFromCapitals = 0x63327363U,

    /// <summary>
    ///     Distances. Shortcode: dist.
    ///     Provides a means to control distance between glyphs.
    /// </summary>
    Distances = 0x64697374U,

    /// <summary>
    ///     Discretionary Ligatures. Shortcode: dlig.
    ///     Replaces a sequence of glyphs with a single glyph which is preferred for typographic purposes.
    ///     This feature covers those ligatures which may be used for special effect, at the user’s preference.
    /// </summary>
    DiscretionaryLigatures = 0x646C6967U,

    /// <summary>
    ///     Denominators. Shortcode: dnom.
    ///     Replaces selected figures which follow a slash with denominator figures.
    /// </summary>
    Denominators = 0x646E6F6DU,

    /// <summary>
    ///     Dotless Forms. Shortcode: dtls.
    ///     This feature provides dotless forms for Math Alphanumeric characters, such as U+1D422 MATHEMATICAL BOLD SMALL I,
    ///     U+1D423 MATHEMATICAL BOLD SMALL J,
    ///     U+1D456 U+MATHEMATICAL ITALIC SMALL I, U+1D457 MATHEMATICAL ITALIC SMALL J, and so on. The dotless forms are to be
    ///     used as base forms for placing mathematical accents over them.
    /// </summary>
    DotlessForms = 0x64746C73U,

    /// <summary>
    ///     Expert Forms. Shortcode: expt.
    ///     Like the JIS78 Forms feature, this feature replaces standard forms in Japanese fonts with corresponding forms
    ///     preferred by typographers.
    ///     Although most of the JIS78 substitutions are included, the expert substitution goes on to handle many more
    ///     characters.
    /// </summary>
    ExpertForms = 0x65787074U,

    /// <summary>
    ///     Final Glyph on Line Alternates. Shortcode: falt.
    ///     Replaces line final glyphs with alternate forms specifically designed for this purpose (they would have less or
    ///     more advance width as need may be), to help justification of text.
    /// </summary>
    FinalGlyphOnLineAlternates = 0x66616C74U,

    /// <summary>
    ///     Terminal Form #2. Shortcode: fin2.
    ///     Replaces the Alaph glyph at the end of Syriac words with its appropriate form, when the preceding base character
    ///     cannot be joined to,
    ///     and that preceding base character is not a Dalath, Rish, or dotless Dalath-Rish.
    /// </summary>
    TerminalForm2 = 0x66696E32U,

    /// <summary>
    ///     Terminal Form #3.  Shortcode: fin3.
    ///     Replaces Alaph glyphs at the end of Syriac words when the preceding base character is a Dalath, Rish, or dotless
    ///     Dalath-Rish.
    /// </summary>
    TerminalForm3 = 0x66696E33U,

    /// <summary>
    ///     Terminal Forms. Shortcode: fina.
    ///     Replaces glyphs for characters that have applicable joining properties with an alternate form when occurring in a
    ///     final context.
    /// </summary>
    TerminalForms = 0x66696E61U,

    /// <summary>
    ///     Flattened ascent forms. Shortcode: flac.
    ///     This feature provides flattened forms of accents to be used over high-rise bases such as capitals.
    ///     This feature should only change the shape of the accent and should not move it in the vertical or horizontal
    ///     direction.
    ///     Moving of the accents is done by the math handling client. Accents are flattened by the Math engine if their base
    ///     is higher than MATH.MathConstants.FlattenedAccentBaseHeight.
    /// </summary>
    FlattenedAscentForms = 0x666C6163U,

    /// <summary>
    ///     Fractions. Shortcode: frac.
    ///     Replaces figures separated by a slash with “common” (diagonal) fractions.
    /// </summary>
    Fractions = 0x66726163U,

    /// <summary>
    ///     Full Widths. Shortcode: fwid.
    ///     Replaces glyphs set on other widths with glyphs set on full (usually em) widths. In a CJKV font, this may include
    ///     “lower ASCII” Latin characters and various symbols.
    ///     In a European font, this feature replaces proportionally-spaced glyphs with monospaced glyphs, which are generally
    ///     set on widths of 0.6 em.
    /// </summary>
    FullWidths = 0x66776964U,

    /// <summary>
    ///     Half Forms. Shortcode: half.
    ///     Produces the half forms of consonants in Indic scripts.
    /// </summary>
    HalfForms = 0x68616C66U,

    /// <summary>
    ///     Halant Forms. Shortcode: haln.
    ///     Produces the halant forms of consonants in Indic scripts.
    /// </summary>
    HalantForms = 0x68616C6EU,

    /// <summary>
    ///     Alternate Half Widths. Shortcode: halt.
    ///     Respaces glyphs designed to be set on full-em widths, fitting them onto half-em widths. This differs from 'hwid' in
    ///     that it does not substitute new glyphs.
    /// </summary>
    AlternateHalfWidths = 0x68616C74U,

    /// <summary>
    ///     Historical Forms. Shortcode: hist.
    ///     Some letterforms were in common use in the past, but appear anachronistic today. The best-known example is the long
    ///     form of s; others would include the old Fraktur k.
    ///     Some fonts include the historical forms as alternates, so they can be used for a “period” effect. This feature
    ///     replaces the default (current) forms with the historical alternates.
    ///     While some ligatures are also used for historical effect, this feature deals only with single characters.
    /// </summary>
    HistoricalForms = 0x68697374U,

    /// <summary>
    ///     Horizontal Kana Alternates. Shortcode: hkna.
    ///     Replaces standard kana with forms that have been specially designed for only horizontal writing. This is a
    ///     typographic optimization for improved fit and more even color. Also see 'vkna'.
    /// </summary>
    HorizontalKanaAlternates = 0x686B6E61U,

    /// <summary>
    ///     Historical Ligatures. Shortcode: hlig.
    ///     Some ligatures were in common use in the past, but appear anachronistic today. Some fonts include the historical
    ///     forms as alternates, so they can be used for a “period” effect.
    ///     This feature replaces the default (current) forms with the historical alternates.
    /// </summary>
    HistoricalLigatures = 0x686C6967U,

    /// <summary>
    ///     Hangul. Shortcode: hngl.
    ///     Replaces hanja (Chinese-style) Korean characters with the corresponding hangul (syllabic) characters. This
    ///     effectively reverses the standard input method,
    ///     in which hangul are entered and replaced by hanja. Many of these substitutions are one-to-one (GSUB lookup type 1),
    ///     but hanja substitution often requires the user to choose from several possible hangul characters (GSUB lookup type
    ///     3).
    /// </summary>
    Hangul = 0x686E676CU,

    /// <summary>
    ///     Hojo Kanji Forms (JIS X 0212-1990 Kanji Forms). Shortcode: hojo.
    ///     The JIS X 0212-1990 (aka, “Hojo Kanji”) and JIS X 0213:2004 character sets overlap significantly.
    ///     In some cases their prototypical glyphs differ. When building fonts that support both JIS X 0212-1990 and JIS X
    ///     0213:2004 (such as those supporting the Adobe-Japan 1-6 character collection),
    ///     it is recommended that JIS X 0213:2004 forms be preferred as the encoded form. The 'hojo' feature is used to access
    ///     the JIS X 0212-1990 glyphs for the cases when the JIS X 0213:2004 form is encoded.
    /// </summary>
    HojoKanjiForms = 0x686F6A6FU,

    /// <summary>
    ///     Half Widths. Shortcode: hwid.
    ///     Replaces glyphs on proportional widths, or fixed widths other than half an em, with glyphs on half-em (en) widths.
    ///     Many CJKV fonts have glyphs which are set on multiple widths; this feature selects the half-em version.
    ///     There are various contexts in which this is the preferred behavior, including compatibility with older desktop
    ///     documents.
    /// </summary>
    HalfWidths = 0x68776964U,

    /// <summary>
    ///     Initial Forms. Shortcode: init.
    ///     Replaces glyphs for characters that have applicable joining properties with an alternate form when occurring in an
    ///     initial context.
    /// </summary>
    InitialForms = 0x696E6974U,

    /// <summary>
    ///     Isolated Forms. Shortcode: isol.
    ///     Replaces glyphs for characters that have applicable joining properties with an alternate form when occurring in a
    ///     isolate (non-joining) context.
    /// </summary>
    IsolatedForms = 0x69736F6CU,

    /// <summary>
    ///     Italics. Shortcode: ital.
    ///     Some fonts (such as Adobe’s Pro Japanese fonts) will have both Roman and Italic forms of some characters in a
    ///     single font.
    ///     This feature replaces the Roman glyphs with the corresponding Italic glyphs.
    /// </summary>
    Italics = 0x6974616CU,

    /// <summary>
    ///     Justification Alternates. Shortcode: jalt.
    ///     Improves justification of text by replacing glyphs with alternate forms specifically designed for this purpose
    ///     (they would have less or more advance width as need may be).
    /// </summary>
    JustificationAlternates = 0x6A616C74U,

    /// <summary>
    ///     JIS78 Forms. Shortcode: jp78.
    ///     This feature replaces default (JIS90) Japanese glyphs with the corresponding forms from the JIS C 6226-1978 (JIS78)
    ///     specification.
    /// </summary>
    Jis78Forms = 0x6A703738U,

    /// <summary>
    ///     JIS83 Forms. Shortcode: jp83.
    ///     This feature replaces default (JIS90) Japanese glyphs with the corresponding forms from the JIS X 0208-1983 (JIS83)
    ///     specification.
    /// </summary>
    Jis83Forms = 0x6A703833U,

    /// <summary>
    ///     JIS90 Forms. Shortcode: jp90.
    ///     This feature replaces Japanese glyphs from the JIS78 or JIS83 specifications with the corresponding forms from the
    ///     JIS X 0208-1990 (JIS90) specification.
    /// </summary>
    Jis90Forms = 0x6A703930U,

    /// <summary>
    ///     JIS2004 Forms. Shortcode: jp04.
    ///     The National Language Council (NLC) of Japan has defined new glyph shapes for a number of JIS characters, which
    ///     were incorporated into JIS X 0213:2004 as new prototypical forms.
    ///     The 'jp04' feature is a subset of the 'nlck' feature, and is used to access these prototypical glyphs in a manner
    ///     that maintains the integrity of JIS X 0213:2004.
    /// </summary>
    Jis2004 = 0x6A703034U,

    /// <summary>
    ///     Kerning. Shortcode: kern.
    ///     Adjusts amount of space between glyphs, generally to provide optically consistent spacing between glyphs.
    ///     Although a well-designed typeface has consistent inter-glyph spacing overall, some glyph combinations require
    ///     adjustment for improved legibility.
    ///     Besides standard adjustment in the horizontal direction, this feature can supply size-dependent kerning data via
    ///     device tables, “cross-stream” kerning in the Y text direction,
    ///     and adjustment of glyph placement independent of the advance adjustment. Note that this feature may apply to runs
    ///     of more than two glyphs, and would not be used in monospaced fonts.
    ///     Also note that this feature does not apply to text set vertically.
    /// </summary>
    Kerning = 0x6B65726EU,

    /// <summary>
    ///     Left Bounds. Shortcode: lfbd.
    ///     Aligns glyphs by their apparent left extents at the left ends of horizontal lines of text, replacing the default
    ///     behavior of aligning glyphs by their origins.
    ///     This feature is called by the Optical Bounds ('opbd') feature.
    /// </summary>
    LeftBounds = 0x6C666264U,

    /// <summary>
    ///     Standard Ligatures. Shortcode: liga.
    ///     Replaces a sequence of glyphs with a single glyph which is preferred for typographic purposes. This feature covers
    ///     the ligatures which the designer/manufacturer judges should be used in normal conditions.
    /// </summary>
    Ligatures = 0x6C696761U,

    /// <summary>
    ///     Leading Jamo Forms. Shortcode: ljmo.
    ///     Substitutes the leading jamo form of a cluster.
    /// </summary>
    LeadingJamoForms = 0x6C6A6D6FU,

    /// <summary>
    ///     Lining Figures. Shortcode: lnum.
    ///     This feature changes selected non-lining figures to lining figures.
    /// </summary>
    LiningFigures = 0x6C6E756DU,

    /// <summary>
    ///     Localized Forms. Shortcode: locl.
    ///     Many scripts used to write multiple languages over wide geographical areas have developed localized variant forms
    ///     of specific letters,
    ///     which are used by individual literary communities. For example, a number of letters in the Bulgarian and Serbian
    ///     alphabets have forms distinct from their Russian counterparts and from each other.
    ///     In some cases the localized form differs only subtly from the script “norm”, in others the forms are radically
    ///     distinct. This feature enables localized forms of glyphs to be substituted for default forms.
    /// </summary>
    LocalizedForms = 0x6C6F636CU,

    /// <summary>
    ///     Left-to-right glyph alternates. Shortcode: ltra.
    ///     This feature applies glyphic variants (other than mirrored forms) appropriate for left-to-right text (for mirrored
    ///     forms, see 'ltrm').
    /// </summary>
    LeftToRightGlyphAlternates = 0x6C747261U,

    /// <summary>
    ///     Left-to-right mirrored forms. Shortcode: ltrm.
    ///     This feature applies mirrored forms appropriate for left-to-right text. (For left-to-right glyph alternates, see
    ///     'ltra').
    /// </summary>
    LeftToRightMirroredForms = 0x6C74726DU,

    /// <summary>
    ///     Mark Positioning. Shortcode: mark.
    ///     Positions mark glyphs with respect to base glyphs.
    /// </summary>
    MarkPositioning = 0x6D61726BU,

    /// <summary>
    ///     Medial Forms #2. Shortcode: med2.
    ///     Replaces Alaph glyphs in the middle of Syriac words when the preceding base character can be joined to.
    /// </summary>
    MedialForms2 = 0x6D656432U,

    /// <summary>
    ///     Medial Forms. Shortcode: medi
    ///     Replaces glyphs for characters that have applicable joining properties with an alternate form when occurring in a
    ///     medial context.
    ///     This applies to characters that have the Unicode Joining_Type property value Dual_Joining.
    /// </summary>
    MedialForms = 0x6D656469U,

    /// <summary>
    ///     Mathematical Greek. Shortcode: mgrk.
    ///     Replaces standard typographic forms of Greek glyphs with corresponding forms commonly used in mathematical notation
    ///     (which are a subset of the Greek alphabet).
    /// </summary>
    MathematicalGreek = 0x6D67726BU,

    /// <summary>
    ///     Mark to Mark Positioning. Shortcode: mkmk.
    ///     Positions marks with respect to other marks. Required in various non-Latin scripts like Arabic.
    /// </summary>
    MarkToMarkPositioning = 0x6D6B6D6BU,

    /// <summary>
    ///     Shortcode: mset.
    ///     Positions Arabic combining marks in fonts for Windows 95 using glyph substitution.
    /// </summary>
    Mset = 0x6D736574U,

    /// <summary>
    ///     Alternate Annotation Forms. Shortcode: nalt.
    ///     Replaces default glyphs with various notational forms (e.g. glyphs placed in open or solid circles, squares,
    ///     parentheses, diamonds or rounded boxes).
    ///     In some cases an annotation form may already be present, but the user may want a different one.
    /// </summary>
    AlternateAnnotationForms = 0x6E616C74U,

    /// <summary>
    ///     NLC Kanji Forms. Shortcode: nlck.
    ///     The National Language Council (NLC) of Japan has defined new glyph shapes for a number of JIS characters in 2000.
    ///     The 'nlck' feature is used to access those glyphs.
    /// </summary>
    NlcKanjiForms = 0x6E6C636BU,

    /// <summary>
    ///     Nukta Forms. Shortcode: nukt.
    ///     Produces Nukta forms in Indic scripts.
    /// </summary>
    NuktaForms = 0x6E756B74U,

    /// <summary>
    ///     Numerators. Shortcode: numr.
    ///     Replaces selected figures which precede a slash with numerator figures, and replaces the typographic slash with the
    ///     fraction slash.
    /// </summary>
    Numerators = 0x6E756D72U,

    /// <summary>
    ///     Oldstyle Figures. Shortcode: onum.
    ///     This feature changes selected figures from the default or lining style to oldstyle form.
    /// </summary>
    OldstyleFigures = 0x6F6E756DU,

    /// <summary>
    ///     Optical Bounds. Shortcode: opbd.
    ///     Aligns glyphs by their apparent left or right extents in horizontal setting, or apparent top or bottom extents in
    ///     vertical setting,
    ///     replacing the default behavior of aligning glyphs by their origins. Another name for this behavior would be visual
    ///     justification.
    ///     The optical edge of a given glyph is only indirectly related to its advance width or bounding box; this feature
    ///     provides a means for getting true visual alignment.
    /// </summary>
    OpticalBounds = 0x6F706264U,

    /// <summary>
    ///     Ordinals. Shortcode: ordn.
    ///     Replaces default alphabetic glyphs with the corresponding ordinal forms for use after figures. One exception to the
    ///     follows-a-figure rule is the numero character (U+2116),
    ///     which is actually a ligature substitution, but is best accessed through this feature.
    /// </summary>
    Ordinals = 0x6F72646EU,

    /// <summary>
    ///     Ornaments. Shortcode: ornm.
    ///     This is a dual-function feature, which uses two input methods to give the user access to ornament glyphs (e.g.
    ///     fleurons, dingbats and border elements) in the font.
    ///     One method replaces the bullet character with a selection from the full set of available ornaments;
    ///     the other replaces specific “lower ASCII” characters with ornaments assigned to them. The first approach supports
    ///     the general or browsing user;
    ///     the second supports the power user.
    /// </summary>
    Ornaments = 0x6F726E6DU,

    /// <summary>
    ///     Proportional Alternate Widths. Shortcode: palt.
    ///     Respaces glyphs designed to be set on full-em widths, fitting them onto individual (more or less proportional)
    ///     horizontal widths.
    ///     This differs from 'pwid' in that it does not substitute new glyphs (GPOS, not GSUB feature). The user may prefer
    ///     the monospaced form,
    ///     or may simply want to ensure that the glyph is well-fit and not rotated in vertical setting (Latin forms designed
    ///     for proportional spacing would be rotated).
    /// </summary>
    ProportionalAlternateWidths = 0x70616C74U,

    /// <summary>
    ///     Petite Capitals. Shortcode: pcap.
    ///     Some fonts contain an additional size of capital letters, shorter than the regular smallcaps and whimsically
    ///     referred to as petite caps.
    ///     Such forms are most likely to be found in designs with a small lowercase x-height, where they better harmonise with
    ///     lowercase text than
    ///     the taller smallcaps (for examples of petite caps, see the Emigre type families Mrs Eaves and Filosofia). This
    ///     feature turns lowercase characters into petite capitals.
    ///     Forms related to petite capitals, such as specially designed figures, may be included.
    /// </summary>
    PetiteCapitals = 0x70636170U,

    /// <summary>
    ///     Proportional Kana. Shortcode: pkna.
    ///     Replaces glyphs, kana and kana-related, set on uniform widths (half or full-width) with proportional glyphs.
    /// </summary>
    ProportionalKana = 0x706B6E61U,

    /// <summary>
    ///     Proportional Figures. Shortcode: pnum.
    ///     Replaces figure glyphs set on uniform (tabular) widths with corresponding glyphs set on glyph-specific
    ///     (proportional) widths.
    ///     Tabular widths will generally be the default, but this cannot be safely assumed. Of course this feature would not
    ///     be present in monospaced designs.
    /// </summary>
    ProportionalFigures = 0x706E756DU,

    /// <summary>
    ///     Pre-base Forms. Shortcode: pref.
    ///     Substitutes the pre-base form of a consonant.
    /// </summary>
    PreBaseForms = 0x70726566U,

    /// <summary>
    ///     Pre-base Substitutions. Shortcode: pres.
    ///     Produces the pre-base forms of conjuncts in Indic scripts. It can also be used to substitute the appropriate glyph
    ///     variant for pre-base vowel signs.
    /// </summary>
    PreBaseSubstitutions = 0x70726573U,

    /// <summary>
    ///     Post-base Forms. Shortcode: pstf.
    ///     Substitutes the post-base form of a consonant.
    /// </summary>
    PostBaseForms = 0x70737466U,

    /// <summary>
    ///     Post-base Substitutions. Shortcode: psts.
    ///     Substitutes a sequence of a base glyph and post-base glyph, with its ligaturised form.
    /// </summary>
    PostBaseSubstitutions = 0x70737473U,

    /// <summary>
    ///     Proportional Widths. Shortcode: pwid.
    ///     Replaces glyphs set on uniform widths (typically full or half-em) with proportionally spaced glyphs.
    ///     The proportional variants are often used for the Latin characters in CJKV fonts, but may also be used for Kana in
    ///     Japanese fonts.
    /// </summary>
    ProportionalWidths = 0x70776964U,

    /// <summary>
    ///     Quarter Widths. Shortcode: qwid.
    ///     Replaces glyphs on other widths with glyphs set on widths of one quarter of an em (half an en). The characters
    ///     involved are normally figures and some forms of punctuation.
    /// </summary>
    QuarterWidths = 0x71776964U,

    /// <summary>
    ///     Randomize. Shortcode: rand.
    ///     In order to emulate the irregularity and variety of handwritten text, this feature allows multiple alternate forms
    ///     to be used.
    /// </summary>
    Randomize = 0x72616E64U,

    /// <summary>
    ///     Required Contextual Alternates. Shortcode: rclt.
    ///     In specified situations, replaces default glyphs with alternate forms which provide for better joining behavior or
    ///     other glyph relationships.
    ///     Especially important in script typefaces which are designed to have some or all of their glyphs join, but
    ///     applicable also to e.g. variants to improve spacing.
    ///     This feature is similar to 'calt', but with the difference that it should not be possible to turn off 'rclt'
    ///     substitutions: they are considered essential to correct layout of the font.
    /// </summary>
    RequiredContextualAlternates = 0x72636C74U,

    /// <summary>
    ///     Required Ligatures. Shortcode: rlig.
    ///     Replaces a sequence of glyphs with a single glyph which is preferred for typographic purposes. This feature covers
    ///     those ligatures, which the script determines as required to be used in normal conditions.
    ///     This feature is important for some scripts to insure correct glyph formation.
    /// </summary>
    RequiredLigatures = 0x726C6967U,

    /// <summary>
    ///     Rakar Forms. Shortcode: rkrf.
    ///     Produces conjoined forms for consonants with rakar in Devanagari and Gujarati scripts.
    /// </summary>
    RakarForms = 0x726B7266U,

    /// <summary>
    ///     Reph Form. Shortcode: rphf.
    ///     Substitutes the Reph form for a consonant and halant sequence.
    /// </summary>
    RephForm = 0x72706866U,

    /// <summary>
    ///     Right Bounds. Shortcode: rtbd.
    ///     Aligns glyphs by their apparent right extents at the right ends of horizontal lines of text, replacing the default
    ///     behavior of aligning glyphs by their origins.
    ///     This feature is called by the Optical Bounds ('opbd') feature.
    /// </summary>
    RightBounds = 0x72746264U,

    /// <summary>
    ///     Right-to-left alternates. Shortcode: rtla.
    ///     This feature applies glyphic variants (other than mirrored forms) appropriate for right-to-left text. (For mirrored
    ///     forms, see 'rtlm'.)
    /// </summary>
    RightToLeftAlternates = 0x72746C61U,

    /// <summary>
    ///     Right-to-left mirrored forms. Shortcode: rtlm.
    ///     This feature applies mirrored forms appropriate for right-to-left text other than for those characters that would
    ///     be covered by the character-level mirroring step performed by an OpenType layout engine.
    ///     (For right-to-left glyph alternates, see 'rtla'.)
    /// </summary>
    RightToLeftMirroredForms = 0x72746C6DU,

    /// <summary>
    ///     Ruby Notation Forms. Shortcode: ruby.
    ///     Japanese typesetting often uses smaller kana glyphs, generally in superscripted form, to clarify the meaning of
    ///     kanji which may be unfamiliar to the reader.
    ///     These are called “ruby”, from the old typesetting term for four-point-sized type. This feature identifies glyphs in
    ///     the font which have been designed for this use,
    ///     substituting them for the default designs.
    /// </summary>
    RubyNotationForms = 0x72756279U,

    /// <summary>
    ///     Required Variation Alternates. Shortcode: rvrn.
    ///     his feature is used in fonts that support OpenType Font Variations in order to select alternate glyphs for
    ///     particular variation instances.
    /// </summary>
    RequiredVariationAlternates = 0x7276726EU,

    /// <summary>
    ///     Stylistic Alternates. Shortcode: salt.
    ///     Many fonts contain alternate glyph designs for a purely esthetic effect; these don’t always fit into a clear
    ///     category like swash or historical.
    ///     As in the case of swash glyphs, there may be more than one alternate form. This feature replaces the default forms
    ///     with the stylistic alternates.
    /// </summary>
    StylisticAlternates = 0x73616C74U,

    /// <summary>
    ///     Scientific Inferiors. Shortcode: sinf.
    ///     Replaces lining or oldstyle figures with inferior figures (smaller glyphs which sit lower than the standard
    ///     baseline, primarily for chemical or mathematical notation).
    ///     May also replace lowercase characters with alphabetic inferiors.
    /// </summary>
    ScientificInferiors = 0x73696E66U,

    /// <summary>
    ///     Optical size. Shortcode: size.
    ///     This feature stores two kinds of information about the optical size of the font: design size
    ///     (the point size for which the font is optimized) and size range (the range of point sizes which the font can serve
    ///     well),
    ///     as well as other information which helps applications use the size range. The design size is useful for determining
    ///     proper tracking behavior.
    ///     The size range is useful in families which have fonts covering several ranges. Additional values serve to identify
    ///     the set of fonts which share related size ranges,
    ///     and to identify their shared name. Note that sizes refer to nominal final output size, and are independent of
    ///     viewing magnification or resolution.
    /// </summary>
    OpticalSize = 0x73697A65U,

    /// <summary>
    ///     Small Capitals. Shortcode: smcp.
    ///     This feature turns lowercase characters into small capitals. This corresponds to the common SC font layout. It is
    ///     generally used for display lines set in Large and small caps, such as titles.
    ///     Forms related to small capitals, such as oldstyle figures, may be included.
    /// </summary>
    SmallCapitals = 0x736D6370U,

    /// <summary>
    ///     Simplified Forms. Shortcode: smpl.
    ///     Replaces “traditional” Chinese or Japanese forms with the corresponding “simplified” forms.
    /// </summary>
    SimplifiedForms = 0x736D706CU,

    /// <summary>
    ///     Math script style alternates. Shortcode: ssty.
    ///     This feature provides glyph variants adjusted to be more suitable for use in subscripts and superscripts.
    /// </summary>
    MathScriptStyleAlternates = 0x73737479U,

    /// <summary>
    ///     Stretching Glyph Decomposition. Shortcode: stch.
    ///     Unicode characters, such as the Syriac Abbreviation Mark (U+070F), that enclose other characters need to be able
    ///     to stretch in order to dynamically adapt to the width of the enclosed text. This feature defines a decomposition
    ///     set
    ///     consisting of an odd number of glyphs which describe the stretching glyph. The odd numbered glyphs in the
    ///     decomposition are
    ///     fixed reference points which are distributed evenly from the start to the end of the enclosed text. The even
    ///     numbered glyphs may
    ///     be repeated as necessary to fill the space between the fixed glyphs. The first and last glyphs may either be simple
    ///     glyphs with width at the baseline,
    ///     or mark glyphs. All other decomposition glyphs should have width, but must be defined as mark glyphs.
    /// </summary>
    StretchingGlyphDecomposition = 0x73746368U,

    /// <summary>
    ///     Subscript. Shortcode: subs.
    ///     The 'subs' feature may replace a default glyph with a subscript glyph, or it may combine a glyph substitution with
    ///     positioning adjustments for proper placement.
    /// </summary>
    Subscript = 0x73756273U,

    /// <summary>
    ///     Superscript. Shortcode: sups.
    ///     Replaces lining or oldstyle figures with superior figures (primarily for footnote indication), and replaces
    ///     lowercase letters with superior letters (primarily for abbreviated French titles).
    /// </summary>
    Superscript = 0x73757073U,

    /// <summary>
    ///     Swash. Shortcode: swsh.
    ///     This feature replaces default character glyphs with corresponding swash glyphs. Note that there may be more than
    ///     one swash alternate for a given character.
    /// </summary>
    Swash = 0x73777368U,

    /// <summary>
    ///     Titling. Shortcode: titl.
    ///     This feature replaces the default glyphs with corresponding forms designed specifically for titling.
    ///     These may be all-capital and/or larger on the body, and adjusted for viewing at larger sizes.
    /// </summary>
    Titling = 0x7469746CU,

    /// <summary>
    ///     Trailing Jamo Forms. Shortcode: tjmo.
    ///     Substitutes the trailing jamo form of a cluster.
    /// </summary>
    TrailingJamoForms = 0x746A6D6FU,

    /// <summary>
    ///     Traditional Name Forms. Shortcode: tnam.
    ///     Replaces “simplified” Japanese kanji forms with the corresponding “traditional” forms. This is equivalent to the
    ///     Traditional Forms feature,
    ///     but explicitly limited to the traditional forms considered proper for use in personal names (as many as 205 glyphs
    ///     in some fonts).
    /// </summary>
    TraditionalNameForms = 0x746E616DU,

    /// <summary>
    ///     Tabular Figures. Shortcode: tnum.
    ///     Replaces figure glyphs set on proportional widths with corresponding glyphs set on uniform (tabular) widths.
    ///     Tabular widths will generally be the default, but this cannot be safely assumed. Of course this feature would not
    ///     be present in monospaced designs.
    /// </summary>
    TabularFigures = 0x746E756DU,

    /// <summary>
    ///     Traditional Forms. Shortcode: trad.
    ///     Replaces 'simplified' Chinese hanzi or Japanese kanji forms with the corresponding 'traditional' forms.
    /// </summary>
    TraditionalForms = 0x74726164U,

    /// <summary>
    ///     Third Widths. Shortcode: twid.
    ///     Replaces glyphs on other widths with glyphs set on widths of one third of an em. The characters involved are
    ///     normally figures and some forms of punctuation.
    /// </summary>
    ThirdWidths = 0x74776964U,

    /// <summary>
    ///     Unicase. Shortcode: unic.
    ///     This feature maps upper- and lowercase letters to a mixed set of lowercase and small capital forms, resulting in a
    ///     single case alphabet
    ///     (for an example of unicase, see the Emigre type family Filosofia). The letters substituted may vary from font to
    ///     font, as appropriate to the design.
    ///     If aligning to the x-height, smallcap glyphs may be substituted, or specially designed unicase forms might be used.
    ///     Substitutions might also include specially designed figures.
    /// </summary>
    Unicase = 0x756E6963U,

    /// <summary>
    ///     Alternate Vertical Metrics. Shortcode: valt.
    ///     Repositions glyphs to visually center them within full-height metrics, for use in vertical setting. Typically
    ///     applies to full-width Latin glyphs,
    ///     which are aligned on a common horizontal baseline and not rotated when set vertically in CJKV fonts.
    /// </summary>
    AlternateVerticalMetrics = 0x76616C74U,

    /// <summary>
    ///     Vattu Variants. Shortcode: vatu.
    ///     In an Indic consonant conjunct, substitutes a ligature glyph for a base consonant and a following vattu
    ///     (below-base) form of a conjoining consonant, or for a half form of a consonant and a following vattu form.
    /// </summary>
    VattuVariants = 0x76617475U,

    /// <summary>
    ///     Vertical Alternates. Shortcode: vert.
    ///     Transforms default glyphs into glyphs that are appropriate for upright presentation in vertical writing mode.While
    ///     the glyphs for most
    ///     characters in East Asian writing systems remain upright when set in vertical writing mode, some must be transformed
    ///     —
    ///     usually by rotation, shifting, or different component ordering — for vertical writing mode.
    /// </summary>
    VerticalAlternates = 0x76657274U,

    /// <summary>
    ///     Alternate Vertical Half Metrics. Shortcode: vhal.
    ///     Respaces glyphs designed to be set on full-em heights, fitting them onto half-em heights.
    /// </summary>
    AlternateVerticalHalfMetrics = 0x7668616CU,

    /// <summary>
    ///     Vowel Jamo Forms. Shortcode: vjmo.
    ///     Substitutes the vowel jamo form of a cluster.
    /// </summary>
    VowelJamoForms = 0x766A6D6FU,

    /// <summary>
    ///     Vertical Kana Alternates. Shortcode: vkna.
    ///     Replaces standard kana with forms that have been specially designed for only vertical writing. This is a
    ///     typographic optimization for improved fit and more even color. Also see 'hkna'.
    /// </summary>
    VerticalKanaAlternates = 0x766B6E61U,

    /// <summary>
    ///     Vertical Kerning. Shortcode: vkrn
    ///     Adjusts amount of space between glyphs, generally to provide optically consistent spacing between glyphs.
    ///     Although a well-designed typeface has consistent inter-glyph spacing overall, some glyph combinations require
    ///     adjustment for improved legibility.
    ///     Besides standard adjustment in the vertical direction, this feature can supply size-dependent kerning data via
    ///     device tables,
    ///     “cross-stream” kerning in the X text direction, and adjustment of glyph placement independent of the advance
    ///     adjustment.
    ///     Note that this feature may apply to runs of more than two glyphs, and would not be used in monospaced fonts. Also
    ///     note that this feature applies only to text set vertically.
    /// </summary>
    VerticalKerning = 0x766B726EU,

    /// <summary>
    ///     Proportional Alternate Vertical Metrics. Shortcode: vpal.
    ///     Respaces glyphs designed to be set on full-em heights, fitting them onto individual (more or less proportional)
    ///     vertical heights. This differs from 'valt' in that it does not substitute new glyphs (GPOS, not GSUB feature).
    ///     The user may prefer the monospaced form, or may simply want to ensure that the glyph is well-fit.
    /// </summary>
    ProportionalAlternateVerticalMetrics = 0x7670616CU,

    /// <summary>
    ///     Vertical Alternates and Rotation. Shortcode: vrt2.
    ///     Replaces some fixed-width (half-, third- or quarter-width) or proportional-width glyphs (mostly Latin or katakana)
    ///     with forms suitable for vertical writing (that is, rotated 90 degrees clockwise).
    ///     Note that these are a superset of the glyphs covered in the 'vert' table.
    /// </summary>
    VerticalAlternatesAndRotation = 0x76727432U,

    /// <summary>
    ///     Vertical Alternates for Rotation. Shortcode: vrtr.
    ///     Transforms default glyphs into glyphs that are appropriate for sideways presentation in vertical writing mode.
    ///     While the glyphs for most characters in East Asian writing systems remain upright when set in vertical writing
    ///     mode, glyphs for other characters —
    ///     such as those of other scripts or for particular Western-style punctuation — are expected to be presented sideways
    ///     in vertical writing.
    /// </summary>
    VerticalAlternatesForRotation = 0x76727472U,

    /// <summary>
    ///     Slashed Zero. Shortcode: zero.
    ///     Some fonts contain both a default form of zero, and an alternative form which uses a diagonal slash through the
    ///     counter. Especially in condensed designs, it can be difficult to distinguish between 0 and O (zero and capital O)
    ///     in any situation where capitals and lining figures may be arbitrarily mixed.
    ///     This feature allows the user to change from the default 0 to a slashed form.
    /// </summary>
    SlashedZero = 0x7A65726FU
}