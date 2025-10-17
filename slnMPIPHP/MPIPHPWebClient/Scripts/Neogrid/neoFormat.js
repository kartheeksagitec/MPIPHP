//neoFormat Plugin
(function ($, undefined) {
    var neoFormat = window.neoFormat = window.neoFormat || {},
      math = Math,
      JSON = window.JSON || {},
      lstrFormatRegExp = /\{(\d+)(:[^\}]+)?\}/g,
      lstrFunctionType = "function",
      lstrStringType = "string",
      lstrNumberType = "number",
      lstrObjectType = "object",
      lstrNullType = "null",
      lstrBooleanType = "boolean",
      lstrUndefinedType = "undefined",
      slice = [].slice,
      globalize = window.Globalize,
     larrzeros = ["", "0", "00", "000", "0000"];
    function numPad(aNumber, aDigits, aEnd) {
        aNumber = aNumber + "";
        aDigits = aDigits || 2;
        aEnd = aDigits - aNumber.length;

        if (aEnd) {
            return larrzeros[aDigits].substring(0, aEnd) + aNumber;
        }

        return aNumber;
    }
    (function () {
        var ESCAPABLE = /[\\\"\x00-\x1f\x7f-\x9f\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g,
          gap,
          indent,
          META = {
              "\b": "\\b",
              "\t": "\\t",
              "\n": "\\n",
              "\f": "\\f",
              "\r": "\\r",
              "\"": '\\"',
              "\\": "\\\\"
          },
          rep,
          toString = {}.toString;
        if (typeof Date.prototype.toJSON !== lstrFunctionType) {
            Date.prototype.toJSON = function (key) {
                var that = this;
                return isFinite(that.valueOf()) ?
                  numPad(that.getUTCFullYear(), 4) + "-" +
                  numPad(that.getUTCMonth() + 1) + "-" +
                  numPad(that.getUTCDate()) + "T" +
                  numPad(that.getUTCHours()) + ":" +
                  numPad(that.getUTCMinutes()) + ":" +
                  numPad(that.getUTCSeconds()) + "Z" : null;
            };
            String.prototype.toJSON = Number.prototype.toJSON = Boolean.prototype.toJSON = function (key) {
                return this.valueOf();
            };
        }

        function quote(string) {
            ESCAPABLE.lastIndex = 0;
            return ESCAPABLE.test(string) ? "\"" + string.replace(ESCAPABLE, function (a) {
                var c = META[a];
                return typeof c === lstrStringType ? c :
                  "\\u" + ("0000" + a.charCodeAt(0).toString(16)).slice(-4);
            }) + "\"" : "\"" + string + "\"";
        }

        function str(key, holder) {
            var i,
              k,
              v,
              length,
              mind = gap,
              partial,
              value = holder[key],
              type;

            if (value && typeof value === lstrObjectType && typeof value.toJSON === lstrFunctionType) {
                value = value.toJSON(key);
            }

            if (typeof rep === lstrFunctionType) {
                value = rep.call(holder, key, value);
            }

            type = typeof value;
            if (type === lstrStringType) {
                return quote(value);
            } else if (type === lstrNumberType) {
                return isFinite(value) ? String(value) : lstrNullType;
            } else if (type === lstrBooleanType || type === lstrNullType) {
                return String(value);
            } else if (type === lstrObjectType) {
                if (!value) {
                    return lstrNullType;
                }
                gap += indent;
                partial = [];
                if (toString.apply(value) === "[object Array]") {
                    length = value.length;
                    for (i = 0; i < length; i++) {
                        partial[i] = str(i, value) || lstrNullType;
                    }
                    v = partial.length === 0 ? "[]" : gap ?
                      "[\n" + gap + partial.join(",\n" + gap) + "\n" + mind + "]" :
                      "[" + partial.join(",") + "]";
                    gap = mind;
                    return v;
                }
                if (rep && typeof rep === lstrObjectType) {
                    length = rep.length;
                    for (i = 0; i < length; i++) {
                        if (typeof rep[i] === lstrStringType) {
                            k = rep[i];
                            v = str(k, value);
                            if (v) {
                                partial.push(quote(k) + (gap ? ": " : ":") + v);
                            }
                        }
                    }
                } else {
                    for (k in value) {
                        if (Object.hasOwnProperty.call(value, k)) {
                            v = str(k, value);
                            if (v) {
                                partial.push(quote(k) + (gap ? ": " : ":") + v);
                            }
                        }
                    }
                }

                v = partial.length === 0 ? "{}" : gap ?
                  "{\n" + gap + partial.join(",\n" + gap) + "\n" + mind + "}" :
                  "{" + partial.join(",") + "}";
                gap = mind;
                return v;
            }
        }

        if (typeof JSON.stringify !== lstrFunctionType) {
            JSON.stringify = function (value, replacer, space) {
                var i;
                gap = "";
                indent = "";

                if (typeof space === lstrNumberType) {
                    for (i = 0; i < space; i += 1) {
                        indent += " ";
                    }

                } else if (typeof space === lstrStringType) {
                    indent = space;
                }

                rep = replacer;
                if (replacer && typeof replacer !== lstrFunctionType && (typeof replacer !== lstrObjectType || typeof replacer.length !== lstrNumberType)) {
                    throw new Error("JSON.stringify");
                }

                return str("", {
                    "": value
                });
            };
        }
    })();
    // Date and Number formatting
    (function () {
        var DATE_FORMAT = /dddd|ddd|dd|d|MMMM|MMM|MM|M|yyyy|yy|HH|H|hh|h|mm|m|fff|ff|f|tt|ss|s|"[^"]*"|'[^']*'/g,
          STANDARD_FORMAT = /^(n|c|p|e)(\d*)$/i,
          LITERAL = /["'].*?["']/g,
          COMMA_REG_EXP = /\,/g,
          EMPTY = "",
          POINT = ".",
          COMMA = ",",
          HASH = "#",
          ZERO = "0",
          PLACEHOLDER = "??",
          EN = "en-US";

        //cultures
        neoFormat.cultures = {
            "en-US": {
                name: EN,
                numberFormat: {
                    pattern: ["-n"],
                    decimals: 2,
                    ",": ",",
                    ".": ".",
                    groupSize: [3],
                    percent: {
                        pattern: ["-n %", "n %"],
                        decimals: 2,
                        ",": ",",
                        ".": ".",
                        groupSize: [3],
                        symbol: "%"
                    },
                    currency: {
                        pattern: ["($n)", "$n"],
                        decimals: 2,
                        ",": ",",
                        ".": ".",
                        groupSize: [3],
                        symbol: "$"
                    }
                },
                calendars: {
                    standard: {
                        days: {
                            names: ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"],
                            namesAbbr: ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"],
                            namesShort: ["Su", "Mo", "Tu", "We", "Th", "Fr", "Sa"]
                        },
                        months: {
                            names: ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"],
                            namesAbbr: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"]
                        },
                        AM: ["AM", "am", "AM"],
                        PM: ["PM", "pm", "PM"],
                        patterns: {
                            d: "M/d/yyyy",
                            D: "dddd, MMMM dd, yyyy",
                            F: "dddd, MMMM dd, yyyy h:mm:ss tt",
                            g: "M/d/yyyy h:mm tt",
                            G: "M/d/yyyy h:mm:ss tt",
                            m: "MMMM dd",
                            M: "MMMM dd",
                            s: "yyyy'-'MM'-'ddTHH':'mm':'ss",
                            t: "h:mm tt",
                            T: "h:mm:ss tt",
                            u: "yyyy'-'MM'-'dd HH':'mm':'ss'Z'",
                            y: "MMMM, yyyy",
                            Y: "MMMM, yyyy"
                        },
                        "/": "/",
                        ":": ":",
                        firstDay: 0
                    }
                }
            }
        };

        function findCulture(culture) {
            if (culture) {
                if (culture.numberFormat) {
                    return culture;
                }

                if (typeof culture === lstrStringType) {
                    var cultures = neoFormat.cultures;
                    return cultures[culture] || cultures[culture.split("-")[0]] || null;
                }

                return null;
            }

            return null;
        }

        function getCulture(culture) {
            if (culture) {
                culture = findCulture(culture);
            }

            return culture || neoFormat.cultures.current;
        }

        neoFormat.culture = function (cultureName) {
            var cultures = neoFormat.cultures,
              culture;

            if (cultureName !== undefined) {
                culture = findCulture(cultureName) || cultures[EN];
                culture.calendar = culture.calendars.standard;
                cultures.current = culture;
            } else {
                return cultures.current;
            }
        };
        neoFormat.findCulture = findCulture;
        neoFormat.getCulture = getCulture;
        //set current culture to en-US.
        neoFormat.culture("en-US");

        function formatDate(date, format, culture) {
            culture = getCulture(culture);

            var calendar = culture.calendars.standard,
              days = calendar.days,
              months = calendar.months;

            format = calendar.patterns[format] || format;

            return format.replace(DATE_FORMAT, function (match) {
                var result;

                if (match === "d") {
                    result = date.getDate();
                } else if (match === "dd") {
                    result = numPad(date.getDate());
                } else if (match === "ddd") {
                    result = days.namesAbbr[date.getDay()];
                } else if (match === "dddd") {
                    result = days.names[date.getDay()];
                } else if (match === "M") {
                    result = date.getMonth() + 1;
                } else if (match === "MM") {
                    result = numPad(date.getMonth() + 1);
                } else if (match === "MMM") {
                    result = months.namesAbbr[date.getMonth()];
                } else if (match === "MMMM") {
                    result = months.names[date.getMonth()];
                } else if (match === "yy") {
                    result = numPad(date.getFullYear() % 100);
                } else if (match === "yyyy") {
                    result = numPad(date.getFullYear(), 4);
                } else if (match === "h") {
                    result = date.getHours() % 12 || 12;
                } else if (match === "hh") {
                    result = numPad(date.getHours() % 12 || 12);
                } else if (match === "H") {
                    result = date.getHours();
                } else if (match === "HH") {
                    result = numPad(date.getHours());
                } else if (match === "m") {
                    result = date.getMinutes();
                } else if (match === "mm") {
                    result = numPad(date.getMinutes());
                } else if (match === "s") {
                    result = date.getSeconds();
                } else if (match === "ss") {
                    result = numPad(date.getSeconds());
                } else if (match === "f") {
                    result = math.floor(date.getMilliseconds() / 100);
                } else if (match === "ff") {
                    result = math.floor(date.getMilliseconds() / 10);
                } else if (match === "fff") {
                    result = date.getMilliseconds();
                } else if (match === "tt") {
                    result = date.getHours() < 12 ? calendar.AM[0] : calendar.PM[0];
                }

                return result !== undefined ? result : match.slice(1, match.length - 1);
            });
        }

        //number formatting
        function formatNumber(number, format, culture) {
            culture = getCulture(culture);

            var numberFormat = culture.numberFormat,
              groupSize = numberFormat.groupSize[0],
              groupSeparator = numberFormat[COMMA],
              decimal = numberFormat[POINT],
              precision = numberFormat.decimals,
              pattern = numberFormat.pattern[0],
              literals = [],
              symbol,
              isCurrency, isPercent,
              customPrecision,
              formatAndPrecision,
              negative = number < 0,
              integer,
              fraction,
              integerLength,
              fractionLength,
              replacement = EMPTY,
              value = EMPTY,
              idx,
              length,
              ch,
              hasGroup,
              hasNegativeFormat,
              decimalIndex,
              sharpIndex,
              zeroIndex,
              percentIndex,
              startZeroIndex,
              start = -1,
              end;

            //return empty string if no number
            if (number === undefined) {
                return EMPTY;
            }

            if (!isFinite(number)) {
                return number;
            }

            //if no format then return number.toString() or number.toLocaleString() if culture.name is not defined
            if (!format) {
                return culture.name.length ? number.toLocaleString() : number.toString();
            }

            formatAndPrecision = STANDARD_FORMAT.exec(format);

            // standard formatting
            if (formatAndPrecision) {
                format = formatAndPrecision[1].toLowerCase();

                isCurrency = format === "c";
                isPercent = format === "p";

                if (isCurrency || isPercent) {
                    //get specific number format information if format is currency or percent
                    numberFormat = isCurrency ? numberFormat.currency : numberFormat.percent;
                    groupSize = numberFormat.groupSize[0];
                    groupSeparator = numberFormat[COMMA];
                    decimal = numberFormat[POINT];
                    precision = numberFormat.decimals;
                    symbol = numberFormat.symbol;
                    pattern = numberFormat.pattern[negative ? 0 : 1];
                }

                customPrecision = formatAndPrecision[2];

                if (customPrecision) {
                    precision = +customPrecision;
                }

                //return number in exponential format
                if (format === "e") {
                    return customPrecision ? number.toExponential(precision) : number.toExponential(); // toExponential() and toExponential(undefined) differ in FF #653438.
                }

                // multiply if format is percent
                if (isPercent) {
                    number *= 100;
                }

                number = number.toFixed(precision);
                number = number.split(POINT);

                integer = number[0];
                fraction = number[1];

                //exclude "-" if number is negative.
                if (negative) {
                    integer = integer.substring(1);
                }

                value = integer;
                integerLength = integer.length;

                //add group separator to the number if it is longer enough
                if (integerLength >= groupSize) {
                    value = EMPTY;
                    for (idx = 0; idx < integerLength; idx++) {
                        if (idx > 0 && (integerLength - idx) % groupSize === 0) {
                            value += groupSeparator;
                        }
                        value += integer.charAt(idx);
                    }
                }

                if (fraction) {
                    value += decimal + fraction;
                }

                if (format === "n" && !negative) {
                    return value;
                }

                number = EMPTY;

                for (idx = 0, length = pattern.length; idx < length; idx++) {
                    ch = pattern.charAt(idx);

                    if (ch === "n") {
                        number += value;
                    } else if (ch === "$" || ch === "%") {
                        number += symbol;
                    } else {
                        number += ch;
                    }
                }

                return number;
            }

            //custom formatting
            //
            //separate format by sections.

            //make number positive
            if (negative) {
                number = -number;
            }

            format = format.split(";");
            if (negative && format[1]) {
                //get negative format
                format = format[1];
                hasNegativeFormat = true;
            } else if (number === 0) {
                //format for larrzeros
                format = format[2] || format[0];
                if (format.indexOf(HASH) == -1 && format.indexOf(ZERO) == -1) {
                    //return format if it is string constant.
                    return format;
                }
            } else {
                format = format[0];
            }

            if (format.indexOf("'") > -1 || format.indexOf("\"") > -1) {
                format = format.replace(LITERAL, function (match) {
                    literals.push(match);
                    return PLACEHOLDER;
                });
            }

            percentIndex = format.indexOf("%");

            isPercent = percentIndex != -1;
            isCurrency = format.indexOf("$") != -1;

            //multiply number if the format has percent
            if (isPercent) {
                if (format[percentIndex - 1] !== "\\") {
                    number *= 100;
                } else {
                    format = format.split("\\").join("");
                }
            }

            if (isCurrency || isPercent) {
                //get specific number format information if format is currency or percent
                numberFormat = isCurrency ? numberFormat.currency : numberFormat.percent;
                groupSize = numberFormat.groupSize[0];
                groupSeparator = numberFormat[COMMA];
                decimal = numberFormat[POINT];
                precision = numberFormat.decimals;
                symbol = numberFormat.symbol;
            }

            hasGroup = format.indexOf(COMMA) > -1;
            if (hasGroup) {
                format = format.replace(COMMA_REG_EXP, EMPTY);
            }

            decimalIndex = format.indexOf(POINT);
            length = format.length;

            if (decimalIndex != -1) {
                zeroIndex = format.lastIndexOf(ZERO);
                sharpIndex = format.lastIndexOf(HASH);
                fraction = number.toString().split(POINT)[1] || EMPTY;

                if (sharpIndex > zeroIndex && fraction.length > (sharpIndex - zeroIndex)) {
                    idx = sharpIndex;
                } else if (zeroIndex != -1 && zeroIndex >= decimalIndex) {
                    idx = zeroIndex;
                }

                if (idx) {
                    number = number.toFixed(idx - decimalIndex);
                }

            } else {
                number = number.toFixed(0);
            }

            sharpIndex = format.indexOf(HASH);
            startZeroIndex = zeroIndex = format.indexOf(ZERO);

            //define the index of the first digit placeholder
            if (sharpIndex == -1 && zeroIndex != -1) {
                start = zeroIndex;
            } else if (sharpIndex != -1 && zeroIndex == -1) {
                start = sharpIndex;
            } else {
                start = sharpIndex > zeroIndex ? zeroIndex : sharpIndex;
            }

            sharpIndex = format.lastIndexOf(HASH);
            zeroIndex = format.lastIndexOf(ZERO);

            //define the index of the last digit placeholder
            if (sharpIndex == -1 && zeroIndex != -1) {
                end = zeroIndex;
            } else if (sharpIndex != -1 && zeroIndex == -1) {
                end = sharpIndex;
            } else {
                end = sharpIndex > zeroIndex ? sharpIndex : zeroIndex;
            }

            if (start == length) {
                end = start;
            }

            if (start != -1) {
                value = number.toString().split(POINT);
                integer = value[0];
                fraction = value[1] || EMPTY;

                integerLength = integer.length;
                fractionLength = fraction.length;

                //add group separator to the number if it is longer enough
                if (hasGroup) {
                    if (integerLength === groupSize && integerLength < decimalIndex - startZeroIndex) {
                        integer = groupSeparator + integer;
                    } else if (integerLength > groupSize) {
                        value = EMPTY;
                        for (idx = 0; idx < integerLength; idx++) {
                            if (idx > 0 && (integerLength - idx) % groupSize === 0) {
                                value += groupSeparator;
                            }
                            value += integer.charAt(idx);
                        }

                        integer = value;
                    }
                }

                number = format.substring(0, start);

                if (negative && !hasNegativeFormat) {
                    number += "-";
                }

                for (idx = start; idx < length; idx++) {
                    ch = format.charAt(idx);

                    if (decimalIndex == -1) {
                        if (end - idx < integerLength) {
                            number += integer;
                            break;
                        }
                    } else {
                        if (zeroIndex != -1 && zeroIndex < idx) {
                            replacement = EMPTY;
                        }

                        if ((decimalIndex - idx) <= integerLength && decimalIndex - idx > -1) {
                            number += integer;
                            idx = decimalIndex;
                        }

                        if (decimalIndex === idx) {
                            number += (fraction ? decimal : EMPTY) + fraction;
                            idx += end - decimalIndex + 1;
                            continue;
                        }
                    }

                    if (ch === ZERO) {
                        number += ch;
                        replacement = ch;
                    } else if (ch === HASH) {
                        number += replacement;
                    }
                }

                if (end >= start) {
                    number += format.substring(end + 1);
                }

                //replace symbol placeholders
                if (isCurrency || isPercent) {
                    value = EMPTY;
                    for (idx = 0, length = number.length; idx < length; idx++) {
                        ch = number.charAt(idx);
                        value += (ch === "$" || ch === "%") ? symbol : ch;
                    }
                    number = value;
                }

                if (literals[0]) {
                    length = literals.length;
                    for (idx = 0; idx < length; idx++) {
                        number = number.replace(PLACEHOLDER, literals[idx]);
                    }
                }
            }

            return number;
        }

        var toString = function (value, fmt, culture) {
            if (fmt) {
                if (value instanceof Date) {
                    return formatDate(value, fmt, culture);
                } else if (typeof value === lstrNumberType) {
                    return formatNumber(value, fmt, culture);
                }
            }

            return value !== undefined ? value : "";
        };

        if (globalize) {
            toString = proxy(globalize.format, globalize);
        }

        neoFormat.format = function (fmt) {
            var values = arguments;

            return fmt.replace(lstrFormatRegExp, function (match, index, placeholderFormat) {
                var value = values[parseInt(index, 10) + 1];

                return toString(value, placeholderFormat ? placeholderFormat.substring(1) : "");
            });
        };

        neoFormat._extractFormat = function (format) {
            if (format.slice(0, 3) === "{0:") {
                format = format.slice(3, format.length - 1);
            }

            return format;
        };

        neoFormat.toString = toString;
    })();
    (function () {
        var NON_BREAKING_SPACE_REG_EXP = /\u00A0/g,
          EXPONENT_REG_EXP = /[eE][\-+]?[0-9]+/,
          SHORT_TIMEZONE_REG_EXP = /[+|\-]\d{1,2}/,
          LONG_TIMEZONE_REG_EXP = /[+|\-]\d{1,2}:\d{2}/,
          DATE_REG_EXP = /^\/Date\((.*?)\)\/$/,
          larrFormatsSequence = ["G", "g", "d", "F", "D", "y", "m", "T", "t"],
          NUMBER_REG_EXP = {
              2: /^\d{1,2}/,
              4: /^\d{4}/
          };

        function outOfRange(aValue, aStart, aEnd) {
            return !(aValue >= aStart && aValue <= aEnd);
        }

        function designatorPredicate(designator) {
            return designator.charAt(0);
        }

        function mapDesignators(designators) {
            return $.map(designators, designatorPredicate);
        }

        //if date's day is different than the typed one - adjust
        function adjustDate(date, hours) {
            if (!hours && date.getHours() === 23) {
                date.setHours(date.getHours() + 2);
            }
        }

        function parseExact(value, format, culture) {
            if (!value) {
                return null;
            }

            var lookAhead = function (match) {
                var i = 0;
                while (format[idx] === match) {
                    i++;
                    idx++;
                }
                if (i > 0) {
                    idx -= 1;
                }
                return i;
            },
              getNumber = function (size) {
                  var rg = NUMBER_REG_EXP[size] || new RegExp('^\\d{1,' + size + '}'),
                    match = value.substr(valueIdx, size).match(rg);

                  if (match) {
                      match = match[0];
                      valueIdx += match.length;
                      return parseInt(match, 10);
                  }
                  return null;
              },
              getIndexByName = function (names) {
                  var i = 0,
                    length = names.length,
                    name, nameLength;

                  for (; i < length; i++) {
                      name = names[i];
                      nameLength = name.length;

                      if (value.substr(valueIdx, nameLength) == name) {
                          valueIdx += nameLength;
                          return i + 1;
                      }
                  }
                  return null;
              },
              checkLiteral = function () {
                  var result = false;
                  if (value.charAt(valueIdx) === format[idx]) {
                      valueIdx++;
                      result = true;
                  }
                  return result;
              },
              calendar = culture.calendars.standard,
              year = null,
              month = null,
              day = null,
              hours = null,
              minutes = null,
              seconds = null,
              milliseconds = null,
              idx = 0,
              valueIdx = 0,
              literal = false,
              date = new Date(),
              shortYearCutOff = 30,
              defaultYear = date.getFullYear(),
              ch, count, length, pattern,
              pmHour, UTC, ISO8601, matches,
              amDesignators, pmDesignators,
              hoursOffset, minutesOffset,
              century;

            if (!format) {
                format = "d"; //shord date format
            }

            //if format is part of the patterns get real format
            pattern = calendar.patterns[format];
            if (pattern) {
                format = pattern;
            }

            format = format.split("");
            length = format.length;

            for (; idx < length; idx++) {
                ch = format[idx];

                if (literal) {
                    if (ch === "'") {
                        literal = false;
                    } else {
                        checkLiteral();
                    }
                } else {
                    if (ch === "d") {
                        count = lookAhead("d");
                        day = count < 3 ? getNumber(2) : getIndexByName(calendar.days[count == 3 ? "namesAbbr" : "names"]);

                        if (day === null || outOfRange(day, 1, 31)) {
                            return null;
                        }
                    } else if (ch === "M") {
                        count = lookAhead("M");
                        month = count < 3 ? getNumber(2) : getIndexByName(calendar.months[count == 3 ? 'namesAbbr' : 'names']);

                        if (month === null || outOfRange(month, 1, 12)) {
                            return null;
                        }
                        month -= 1; //because month is zero based
                    } else if (ch === "y") {
                        count = lookAhead("y");
                        year = getNumber(count);

                        if (year === null) {
                            return null;
                        }

                        if (count == 2) {
                            century = defaultYear - defaultYear % 100;
                            if (shortYearCutOff < year) {
                                century -= 100;
                            }
                            year = century + year;
                        }
                    } else if (ch === "h") {
                        lookAhead("h");
                        hours = getNumber(2);
                        if (hours == 12) {
                            hours = 0;
                        }
                        if (hours === null || outOfRange(hours, 0, 11)) {
                            return null;
                        }
                    } else if (ch === "H") {
                        lookAhead("H");
                        hours = getNumber(2);
                        if (hours === null || outOfRange(hours, 0, 23)) {
                            return null;
                        }
                    } else if (ch === "m") {
                        lookAhead("m");
                        minutes = getNumber(2);
                        if (minutes === null || outOfRange(minutes, 0, 59)) {
                            return null;
                        }
                    } else if (ch === "s") {
                        lookAhead("s");
                        seconds = getNumber(2);
                        if (seconds === null || outOfRange(seconds, 0, 59)) {
                            return null;
                        }
                    } else if (ch === "f") {
                        count = lookAhead("f");
                        milliseconds = getNumber(count);

                        if (milliseconds !== null && count > 3) {
                            milliseconds = parseInt(milliseconds.toString().substring(0, 3), 10);
                        }

                        if (milliseconds === null || outOfRange(milliseconds, 0, 999)) {
                            return null;
                        }

                    } else if (ch === "t") {
                        count = lookAhead("t");
                        amDesignators = calendar.AM;
                        pmDesignators = calendar.PM;

                        if (count === 1) {
                            amDesignators = mapDesignators(amDesignators);
                            pmDesignators = mapDesignators(pmDesignators);
                        }

                        pmHour = getIndexByName(pmDesignators);
                        if (!pmHour && !getIndexByName(amDesignators)) {
                            return null;
                        }
                    } else if (ch === "z") {
                        UTC = true;
                        count = lookAhead("z");

                        if (value.substr(valueIdx, 1) === "Z") {
                            if (!ISO8601) {
                                return null;
                            }

                            checkLiteral();
                            continue;
                        }

                        matches = value.substr(valueIdx, 6)
                          .match(count > 2 ? LONG_TIMEZONE_REG_EXP : SHORT_TIMEZONE_REG_EXP);

                        if (!matches) {
                            return null;
                        }

                        matches = matches[0];
                        valueIdx = matches.length;
                        matches = matches.split(":");

                        hoursOffset = parseInt(matches[0], 10);
                        if (outOfRange(hoursOffset, -12, 13)) {
                            return null;
                        }

                        if (count > 2) {
                            minutesOffset = parseInt(matches[1], 10);
                            if (isNaN(minutesOffset) || outOfRange(minutesOffset, 0, 59)) {
                                return null;
                            }
                        }
                    } else if (ch === "T") {
                        ISO8601 = checkLiteral();
                    } else if (ch === "'") {
                        literal = true;
                        checkLiteral();
                    } else if (!checkLiteral()) {
                        return null;
                    }
                }
            }

            if (year === null) {
                year = defaultYear;
            }

            if (pmHour && hours < 12) {
                hours += 12;
            }

            if (day === null) {
                day = 1;
            }

            if (UTC) {
                if (hoursOffset) {
                    hours += -hoursOffset;
                }

                if (minutesOffset) {
                    minutes += -minutesOffset;
                }

                value = new Date(Date.UTC(year, month, day, hours, minutes, seconds, milliseconds));
            } else {
                value = new Date(year, month, day, hours, minutes, seconds, milliseconds);
                adjustDate(value, hours);
            }

            if (year < 100) {
                value.setFullYear(year);
            }

            return value;
        }

        neoFormat._adjustDate = adjustDate;

        neoFormat.parseDate = function (value, formats, culture) {
            if (value instanceof Date) {
                return value;
            }

            var idx = 0,
              date = null,
              length, patterns;

            if (value && value.indexOf("/D") === 0) {
                date = DATE_REG_EXP.exec(value);
                if (date) {
                    return new Date(parseInt(date[1], 10));
                }
            }

            culture = neoFormat.getCulture(culture);

            if (!formats) {
                formats = [];
                patterns = culture.calendar.patterns;
                length = larrFormatsSequence.length;

                for (; idx < length; idx++) {
                    formats[idx] = patterns[larrFormatsSequence[idx]];
                }
                formats[idx] = "ddd MMM dd yyyy HH:mm:ss";
                formats[++idx] = "yyyy-MM-ddTHH:mm:ss.fffffffzzz";
                formats[++idx] = "yyyy-MM-ddTHH:mm:ss.fffzzz";
                formats[++idx] = "yyyy-MM-ddTHH:mm:sszzz";
                formats[++idx] = "yyyy-MM-ddTHH:mmzzz";
                formats[++idx] = "yyyy-MM-ddTHH:mmzz";
                formats[++idx] = "yyyy-MM-dd";

                idx = 0;
            }

            formats = $.isArray(formats) ? formats : [formats];
            length = formats.length;

            for (; idx < length; idx++) {
                date = parseExact(value, formats[idx], culture);
                if (date) {
                    return date;
                }
            }

            return date;
        };

        neoFormat.parseInt = function (value, culture) {
            var result = neoFormat.parseFloat(value, culture);
            if (result) {
                result = result | 0;
            }
            return result;
        };

        neoFormat.parseFloat = function (value, culture, format) {
            if (!value && value !== 0) {
                return null;
            }

            if (typeof value === lstrNumberType) {
                return value;
            }

            value = value.toString();
            culture = neoFormat.getCulture(culture);

            var number = culture.numberFormat,
              percent = number.percent,
              currency = number.currency,
              symbol = currency.symbol,
              percentSymbol = percent.symbol,
              negative = value.indexOf("-") > -1,
              parts, isPercent;

            //handle exponential number
            if (EXPONENT_REG_EXP.test(value)) {
                value = parseFloat(value);
                if (isNaN(value)) {
                    value = null;
                }
                return value;
            }

            if (value.indexOf(symbol) > -1 || (format && format.toLowerCase().indexOf("c") > -1)) {
                number = currency;
                parts = number.pattern[0].replace("$", symbol).split("n");
                if (value.indexOf(parts[0]) > -1 && value.indexOf(parts[1]) > -1) {
                    value = value.replace(parts[0], "").replace(parts[1], "");
                    negative = true;
                }
            } else if (value.indexOf(percentSymbol) > -1) {
                isPercent = true;
                number = percent;
                symbol = percentSymbol;
            }

            value = value.replace("-", "")
              .replace(symbol, "")
              .replace(NON_BREAKING_SPACE_REG_EXP, " ")
              .split(number[","].replace(NON_BREAKING_SPACE_REG_EXP, " ")).join("")
              .replace(number["."], ".");

            value = parseFloat(value);

            if (isNaN(value)) {
                value = null;
            } else if (negative) {
                value *= -1;
            }

            if (value && isPercent) {
                value /= 100;
            }

            return value;
        };

        if (globalize) {
            neoFormat.parseDate = function (value, format, culture) {
                if (value instanceof Date) {
                    return value;
                }

                return globalize.parseDate(value, format, culture);
            };

            neoFormat.parseFloat = function (value, culture, format) {
                if (typeof value === lstrNumberType) {
                    return value;
                }

                return globalize.parseFloat(value, culture);
            };
        }
    })();
    neoFormat.isNodeEmpty = function (adomElement) {
        return $.trim($(adomElement).contents().filter(function () {
            return this.nodeType != 8;
        }).html()) === "";
    }
    neoFormat.htmlEncode = function (aValue) {
        var lampRegExp = /&/g,
        lltRegExp = /</g,
        lgtRegExp = />/g;
        return ("" + aValue).replace(lampRegExp, "&amp;").replace(lltRegExp, "&lt;").replace(lgtRegExp, "&gt;");
    };
    neoFormat.days = {
        Sunday: 0,
        Monday: 1,
        Tuesday: 2,
        Wednesday: 3,
        Thursday: 4,
        Friday: 5,
        Saturday: 6
    };
    neoFormat.replaceAll = function (Inputstring, SearchValue, ReplaceValue, CaseSensitive) {
        if (CaseSensitive === undefined) {
            CaseSensitive = false;
        }
        var regex;
        if (CaseSensitive == true) {
            regex = new RegExp(['(', SearchValue, ')'].join(''), 'g');
        }
        else {
            regex = new RegExp(['(', SearchValue, ')'].join(''), 'gi');
        }
        return Inputstring.replace(regex, ReplaceValue);
    }
    neoFormat.FormatValue = function (val, format) {
        if (val == "undefined" || val == undefined || val == "") {
            return "";
        }
        var char, code, newValue = "";
        var cnt = 0;
        var i;
        for (i in format) {
            char = format[i];
            code = char.charCodeAt(0);
            if ((code >= 48 && code <= 57) || (code >= 65 && code <= 91) || (code >= 97 && code <= 122) || (char == nsConstants.HASH)) {
                cnt++;
            }
        }
        if (cnt !== ([val, ""].join('')).length) {
            while (cnt > ([val, ""].join('')).length) {
                val = ["0", val].join('');
            }
        }
        cnt = 0;
        for (i in format) {
            char = format[i];
            code = char.charCodeAt(0);

            if ((code >= 48 && code <= 56) || (code >= 65 && code <= 91) || (code >= 97 && code <= 122)) {
                newValue += char;
                cnt++;
            }
            else if ((char == '#') || (char == "9")) {
                newValue += val[cnt];
                cnt++;
            }
            else {
                newValue += char;
            }
        }

        return newValue;
    };
    neoFormat.GetFormatedValue = function (format, value) {
        var lformattedValue = value;
        if (format == undefined) {
            return lformattedValue;
        }
        if (lformattedValue === null || lformattedValue === "") {
            lformattedValue = "";
            return lformattedValue;
        }
        format = neoFormat.replaceAll(format, "X~X", "\\\\#");
        switch (format) {
            case "{0:C}":
            case "{0:c}":
                if (!isNaN(lformattedValue)) {
                    lformattedValue = neoFormat.toString(lformattedValue * 1, "c");
                }
                break;
            case "{0:#0.00'%}":
            case "{0:\\#0.00'%}":
            case "{0:\\\\#0.00'%}":
                if (!isNaN(lformattedValue)) {
                    lformattedValue = neoFormat.toString(lformattedValue * 1, "0.00\\%");
                }
                break;
            case "{0:#0.000'%}":
            case "{0:\\#0.000'%}":
            case "{0:\\\\#0.000'%}":
                if (!isNaN(lformattedValue)) {
                    lformattedValue = neoFormat.toString(lformattedValue * 1, "0.000\\%");
                }
                break;
            case "{0:#0.0000'%}":
            case "{0:\\#0.0000'%}":
            case "{0:\\\\#0.0000'%}":
                if (!isNaN(lformattedValue)) {
                    lformattedValue = neoFormat.toString(lformattedValue * 1, "0.0000\\%");
                }
                break;
            case "{0:#0.00000'%}":
            case "{0:\\#0.00000'%}":
            case "{0:\\\\#0.00000'%}":
                if (!isNaN(lformattedValue)) {
                    lformattedValue = neoFormat.toString(lformattedValue * 1, "0.00000\\%");
                }
                break;
            case "{0:P}":
            case "{0:p}":
                if (!isNaN(lformattedValue)) {
                    lformattedValue = neoFormat.toString(lformattedValue * 1, "# \\%");
                }
                break;
            case "{0:MM/dd/yyyy}":
            case "{0:d}":
                lformattedValue = neoFormat.format("{0:MM/dd/yyyy}", lformattedValue);
                break;
            case "{0:000-##-####}":
            case "{0:000-\\#\\#-\\#\\#\\#\\#}":
            case "{0:000-\\\\#\\\\#-\\\\#\\\\#\\\\#\\\\#}":
                if (lformattedValue != "") {
                    if (lformattedValue.indexOf("-") > 0) {
                        return lformattedValue;
                    }
                    lformattedValue = neoFormat.FormatValue(lformattedValue, "999-99-9999");
                }
                break;
            case "{0:(###)###-####}":
            case "{0:(\\#\\#\\#)\\#\\#\\#-\\#\\#\\#\\#}":
            case "{0:(\\\\#\\\\#\\\\#)\\\\#\\\\#\\\\#-\\\\#\\\\#\\\\#\\\\#}":
                if (lformattedValue != "") {
                    if (lformattedValue.indexOf("-") > 0) {
                        return lformattedValue;
                    }
                    lformattedValue = neoFormat.FormatValue(lformattedValue, "(999)999-9999");
                }
                break;
                //SIS FIEN Format
            case "{0:00-#######}":
            case "{0:00-\\#\\#\\#\\#\\#\\#\\#}":
            case "{0:00-\\\\#\\\\#\\\\#\\\\#\\\\#\\\\#\\\\#}":
                if (lformattedValue != "") {
                    if (lformattedValue.indexOf("-") > 0) {
                        return lformattedValue;
                    }
                    lformattedValue = neoFormat.FormatValue(lformattedValue, "99-9999999");
                }
                break;
            default:
                if (format != "" && format != undefined) {
                    if (!isNaN(lformattedValue)) {
                        if (format.indexOf("{0:") == 0) {
                            format = format.replace("{0:", "").replace("}", "");
                            lformattedValue = neoFormat.toString(lformattedValue * 1, format);
                        }
                    }
                }
                break;
        }
        return lformattedValue;
    };
})(jQuery);