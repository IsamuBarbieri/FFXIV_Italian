var tasks = __TASKS_JSON__;
var styleFile = new File(__STYLE_PATH__);
var referencePsdFile = new File(__REFERENCE_PSD_PATH__);
var styleName = __STYLE_NAME__;
var forceStyleLoad = __RELOAD_STYLE__;
var loadedStyleNames = [];
var punctuationFontPostScriptName = 'JupiterPro-Bold';
var previousDialogs = app.displayDialogs;
app.displayDialogs = DialogModes.NO;
function s(k) { return stringIDToTypeID(k); }
function applyStyleIndex(index) {
    var desc = new ActionDescriptor(), styleRef = new ActionReference(), layerRef = new ActionReference();
    styleRef.putIndex(s('style'), index);
    layerRef.putEnumerated(s('layer'), s('ordinal'), s('targetEnum'));
    desc.putReference(s('null'), styleRef);
    desc.putReference(s('to'), layerRef);
    executeAction(s('applyStyle'), desc, DialogModes.NO);
}
function getStyleNames() {
    var ref = new ActionReference();
    ref.putProperty(s('property'), s('presetManager'));
    ref.putEnumerated(s('application'), s('ordinal'), s('targetEnum'));
    var list = executeActionGet(ref).getList(s('presetManager'));
    var names = list.getObjectValue(3).getList(s('name')), result = [];
    for (var i = 0; i < names.count; i++) result.push(names.getString(i));
    loadedStyleNames = result;
    return result;
}
function hasStyleName(styleName) {
    var names = getStyleNames();
    for (var i = 0; i < names.length; i++) if (names[i] === styleName) return true;
    return false;
}
function findStyleIndex(styleName) {
    var names = getStyleNames();
    var i = names.length - 1;
    while (i >= 0 && names[i] !== styleName) i--;
    if (i < 0) throw new Error('Preset ' + styleName + ' non presente. Stili disponibili: ' + names.join(' | '));
    var candidate = i % 2 === 0 ? i + 1 : i;
    applyStyleIndex(candidate);
    var keys = layerStyleKeys();
    if (keys.indexOf('solidFillMulti') < 0 || keys.indexOf('gradientFillMulti') < 0 ||
        keys.indexOf('bevelEmboss') < 0 || keys.indexOf('outerGlow') < 0)
        throw new Error('Il preset ' + styleName + ' importato non ha la struttura attesa: ' + keys);
    return candidate;
}
function copy(d) { var n = new ActionDescriptor(); n.fromStream(d.toStream()); return n; }
function isPunctuation(ch) {
    return !/\s/.test(ch) && !/[0-9]/.test(ch) && ch.toLowerCase() === ch.toUpperCase();
}
function hasPunctuation(text) {
    for (var i = 0; i < text.length; i++) if (isPunctuation(text.charAt(i))) return true;
    return false;
}
function findFullScreenSourceDocument() {
    var source = null;
    for (var i = 0; i < app.documents.length; i++)
        if (app.documents[i].name === referencePsdFile.name) source = app.documents[i];
    if (!source) {
        if (!referencePsdFile.exists) throw new Error('PSD di riferimento non trovato: ' + referencePsdFile.fsName);
        source = app.open(referencePsdFile);
    }
    return source;
}
function fullScreenTextLayer(doc, task) {
    var layer = doc.artLayers.getByName('This Uses Jupiter Font');
    for (var i = 0; i < doc.layers.length; i++) doc.layers[i].visible = false;
    layer.visible = true;
    doc.activeLayer = layer;
    var sourceText = current().getObjectValue(s('textKey'));
    var sourceRanges = sourceText.getList(s('textStyleRange'));
    if (sourceRanges.count < 2) throw new Error('Il livello PSD non contiene i formati maiuscolo/minuscolo attesi.');
    var upper = copy(sourceRanges.getObjectValue(0).getObjectValue(s('textStyle')));
    var lower = copy(sourceRanges.getObjectValue(1).getObjectValue(s('textStyle')));
    layer.textItem.contents = task.text;
    var textData = current().getObjectValue(s('textKey'));
    var ranges = new ActionList(), scale = 1.7 * task.scale / 3;
    for (var i = 0; i < task.text.length; i++) {
        var ch = task.text.charAt(i);
        var isLowercase = ch !== ch.toUpperCase();
        var style = copy(isLowercase ? lower : upper);
        if (isPunctuation(ch)) style.putString(s('fontPostScriptName'), punctuationFontPostScriptName);
        var size = style.getUnitDoubleValue(s('size')) * scale;
        style.putUnitDouble(s('size'), s('pointsUnit'), size);
        style.putUnitDouble(s('impliedFontSize'), s('pointsUnit'), size);
        var range = new ActionDescriptor();
        range.putInteger(s('from'), i);
        range.putInteger(s('to'), i + 1);
        range.putObject(s('textStyle'), s('textStyle'), style);
        ranges.putObject(s('textStyleRange'), range);
    }
    textData.putList(s('textStyleRange'), ranges);
    var set = new ActionDescriptor(), ref = new ActionReference();
    ref.putEnumerated(s('textLayer'), s('ordinal'), s('targetEnum'));
    set.putReference(s('null'), ref);
    set.putObject(s('to'), s('textLayer'), textData);
    executeAction(s('set'), set, DialogModes.NO);
    var b = layer.bounds, w = b[2].as('px') - b[0].as('px');
    if (w > 1200 * task.scale) throw new Error('Testo oltre la larghezza utile: ' + w + ' px.');
    var h = b[3].as('px') - b[1].as('px');
    var targetHeight = task.height * task.scale;
    doc.resizeCanvas(1280 * task.scale, targetHeight, AnchorPosition.MIDDLECENTER);
    b = layer.bounds;
    w = b[2].as('px') - b[0].as('px');
    h = b[3].as('px') - b[1].as('px');
    layer.translate(new UnitValue((doc.width.as('px') - w) / 2 - b[0].as('px'), 'px'),
        new UnitValue((targetHeight - h) / 2 - b[1].as('px'), 'px'));
    return layer;
}
function current() {
    var r = new ActionReference();
    r.putEnumerated(s('layer'), s('ordinal'), s('targetEnum'));
    return executeActionGet(r);
}
function setRegionGlow(scale) {
    var effects = current().getObjectValue(s('layerEffects'));
    var glow = copy(effects.getObjectValue(s('outerGlow')));
    var factor = scale / 3;
    glow.putUnitDouble(s('chokeMatte'), s('pixelsUnit'), 11 * factor);
    glow.putUnitDouble(s('blur'), s('pixelsUnit'), 22.3 * factor);
    effects.putObject(s('outerGlow'), s('outerGlow'), glow);
    var desc = new ActionDescriptor(), ref = new ActionReference();
    ref.putEnumerated(s('layer'), s('ordinal'), s('targetEnum'));
    desc.putReference(s('null'), ref);
    desc.putObject(s('to'), s('layerEffects'), effects);
    executeAction(s('set'), desc, DialogModes.NO);
}
function setFullScreenGlow(scale) {
    var effects = current().getObjectValue(s('layerEffects'));
    var glow = copy(effects.getObjectValue(s('outerGlow')));
    var factor = scale / 3;
    glow.putUnitDouble(s('chokeMatte'), s('pixelsUnit'), 0);
    glow.putUnitDouble(s('blur'), s('pixelsUnit'), 10 * factor);
    glow.putUnitDouble(s('opacity'), s('percentUnit'), 70);
    var color = new ActionDescriptor();
    color.putDouble(s('red'), 255);
    color.putDouble(s('green'), 184);
    color.putDouble(s('blue'), 65);
    glow.putObject(s('color'), s('RGBColor'), color);
    effects.putObject(s('outerGlow'), s('outerGlow'), glow);
    var desc = new ActionDescriptor(), ref = new ActionReference();
    ref.putEnumerated(s('layer'), s('ordinal'), s('targetEnum'));
    desc.putReference(s('null'), ref);
    desc.putObject(s('to'), s('layerEffects'), effects);
    executeAction(s('set'), desc, DialogModes.NO);
}
function textMask(doc, task) {
    var layer = doc.artLayers.add();
    layer.kind = LayerKind.TEXT;
    var text = task.text, scale = task.scale;
    var canvasWidth = 1024;
    var canvasHeight = task.layout === 'region' ? 64 : 128;
    var size = (task.layout === 'region' ? 26 : 66.3606557377) * scale;
    var fontName = 'JupiterPro';
    var spacing = task.layout === 'region' ? 11 * scale : 0;
    var t = layer.textItem;
    t.contents = text;
    t.font = fontName;
    if (t.font !== fontName) throw new Error('Font richiesto non disponibile: ' + fontName);
    t.size = new UnitValue(size, 'pt');
    t.position = [new UnitValue(100 * scale, 'px'), new UnitValue(80 * scale, 'px')];
    t.capitalization = TextCase.NORMAL;
    t.antiAliasMethod = AntiAlias.SHARP;
    var fillColor = new SolidColor(); fillColor.rgb.red = fillColor.rgb.green = fillColor.rgb.blue = 255;
    t.color = fillColor;
    var td = current().getObjectValue(s('textKey'));
    var base = td.getList(s('textStyleRange')).getObjectValue(0).getObjectValue(s('textStyle'));
    var ranges = new ActionList();
    for (var i = 0; i < text.length; i++) {
        var range = new ActionDescriptor(), style = copy(base);
        var isLowercase = text.charAt(i) !== text.charAt(i).toUpperCase();
        var sz = isLowercase ? size * 5 / 6 : size;
        style.putString(s('fontPostScriptName'), fontName);
        style.putUnitDouble(s('size'), s('pointsUnit'), sz);
        style.putUnitDouble(s('impliedFontSize'), s('pointsUnit'), sz);
        style.putInteger(s('tracking'), Math.round(1000 * spacing / sz));
        style.putBoolean(s('contextualLigatures'), text.charAt(i) !== 'n');
        range.putInteger(s('from'), i);
        range.putInteger(s('to'), i === text.length - 1 ? i + 2 : i + 1);
        range.putObject(s('textStyle'), s('textStyle'), style);
        ranges.putObject(s('textStyleRange'), range);
    }
    td.putList(s('textStyleRange'), ranges);
    var set = new ActionDescriptor(), ref = new ActionReference();
    ref.putEnumerated(s('textLayer'), s('ordinal'), s('targetEnum'));
    set.putReference(s('null'), ref); set.putObject(s('to'), s('textLayer'), td);
    executeAction(s('set'), set, DialogModes.NO);
    var b = layer.bounds, w = b[2].as('px') - b[0].as('px');
    if (w > 940 * scale) {
        layer.resize(940 * scale / w * 100, 940 * scale / w * 100, AnchorPosition.MIDDLECENTER);
        b = layer.bounds; w = b[2].as('px') - b[0].as('px');
    }
    layer.translate(new UnitValue((canvasWidth * scale - w) / 2 - b[0].as('px'), 'px'),
        new UnitValue((task.layout === 'region' ? 23 : 19) * scale - b[1].as('px'), 'px'));
    layer.rasterize(RasterizeType.TEXTCONTENTS);
    if (task.layout === 'region') {
        var left = (1024 * scale - w) / 2 - 3 * scale;
        var line = doc.artLayers.add(); line.name = 'Underline';
        doc.selection.select([[left, 52.5 * scale], [left + w + 6 * scale, 52.5 * scale],
            [left + w + 6 * scale, 53.75 * scale], [left, 53.75 * scale]]);
        doc.selection.fill(fillColor); doc.selection.deselect();
        line.merge();
    }
}
function layerStyleKeys() {
    var ref = new ActionReference();
    ref.putEnumerated(s('layer'), s('ordinal'), s('targetEnum'));
    var layer = executeActionGet(ref), key = s('layerEffects');
    if (!layer.hasKey(key)) return 'none';
    var effects = layer.getObjectValue(key), result = [];
    for (var i = 0; i < effects.count; i++) result.push(typeIDToStringID(effects.getKey(i)));
    return result.join(',');
}
var generationError = null;
try {
    // Check every face before rendering so Photoshop cannot silently substitute a font.
    for (var ti = 0; ti < tasks.length; ti++) {
        var requiredFont = tasks[ti].layout === 'full' ? 'FONTSPRINGDEMO-JupiterProBold' : 'JupiterPro';
        var found = false;
        for (var fi = 0; fi < app.fonts.length; fi++) if (app.fonts[fi].postScriptName === requiredFont) found = true;
        if (!found) throw new Error('Font non disponibile: ' + requiredFont);
        if (tasks[ti].layout === 'full' && hasPunctuation(tasks[ti].text)) {
            found = false;
            for (var fi = 0; fi < app.fonts.length; fi++) if (app.fonts[fi].postScriptName === punctuationFontPostScriptName) found = true;
            if (!found) throw new Error('Font non disponibile: ' + punctuationFontPostScriptName);
        }
    }
    var fullScreenSourceDocument = null;
    for (var t = 0; t < tasks.length; t++) if (tasks[t].layout === 'full') {
        fullScreenSourceDocument = findFullScreenSourceDocument();
        break;
    }
    var stylesLoaded = false;
    var styleIndex = -1;
    for (var i = 0; i < tasks.length; i++) {
        var task = tasks[i], isFullScreen = task.layout === 'full';
        var doc = null;
        var phase = 'create mask';
        try {
            if (isFullScreen) {
                phase = 'duplicate PSD text style';
                doc = fullScreenSourceDocument.duplicate('Area texture ' + (i + 1), false);
            } else {
                doc = app.documents.add(1024 * task.scale,
                    (task.layout === 'zone' ? 128 : 64) * task.scale, 72,
                    'Area texture ' + (i + 1), NewDocumentMode.RGB, DocumentFill.TRANSPARENT);
            }
            if (!isFullScreen && !stylesLoaded) {
                phase = 'check ' + styleName + ' preset';
                if (forceStyleLoad || !hasStyleName(styleName)) {
                    phase = 'load ' + styleName + ' ASL';
                    var styleLoad = new ActionDescriptor(), target = new ActionReference();
                    target.putProperty(charIDToTypeID('Prpr'), charIDToTypeID('Styl'));
                    target.putEnumerated(charIDToTypeID('capp'), charIDToTypeID('Ordn'), charIDToTypeID('Trgt'));
                    styleLoad.putReference(charIDToTypeID('null'), target);
                    styleLoad.putPath(charIDToTypeID('T   '), styleFile);
                    styleLoad.putBoolean(charIDToTypeID('Appe'), true);
                    executeAction(charIDToTypeID('setd'), styleLoad, DialogModes.NO);
                }
                stylesLoaded = true;
            }
            if (isFullScreen) {
                fullScreenTextLayer(doc, task);
            } else {
                textMask(doc, task);
                phase = 'save mask';
                doc.saveAs(new File(task.maskPath), new PNGSaveOptions(), true, Extension.LOWERCASE);
                phase = 'apply ' + styleName;
                if (styleIndex < 0) styleIndex = findStyleIndex(styleName);
                else applyStyleIndex(styleIndex);
            }
            var effectScale = (isFullScreen ? 1.7 : 1) * task.scale / 3;
            if (effectScale !== 1) {
                phase = 'scale effects';
                var fxScale = new ActionDescriptor();
                fxScale.putUnitDouble(s('scale'), s('percentUnit'), 100 * effectScale);
                executeAction(s('scaleEffectsEvent'), fxScale, DialogModes.NO);
            }
            if (isFullScreen) {
                phase = 'tune full screen glow';
                setFullScreenGlow(task.scale);
            } else if (task.layout === 'region') {
                phase = 'scale region glow';
                setRegionGlow(task.scale);
            }
            phase = 'save styled texture';
            doc.saveAs(new File(task.styledPath), new PNGSaveOptions(), true, Extension.LOWERCASE);
        } catch (taskError) {
            throw new Error('Texture ' + (i + 1) + '/' + tasks.length + ' (' + task.text + ', fase ' + phase + '): ' + taskError.message);
        } finally { if (doc) doc.close(SaveOptions.DONOTSAVECHANGES); }
    }
} catch (error) { generationError = error; }
finally { app.displayDialogs = previousDialogs; }
if (generationError) 'ERROR: ' + generationError.message;
else (tasks[0].layout === 'full' ? 'FONTSPRING DEMO Jupiter Pro Bold + stile PSD' : 'Jupiter Pro Regular + ' + styleName) + ': ' + tasks.length + ' texture esportate.';
