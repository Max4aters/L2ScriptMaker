﻿using System.Collections.Generic;
using System.Linq;
using L2ScriptMaker.Core;
using L2ScriptMaker.Models.Ai;

namespace L2ScriptMaker.Parsers
{
	internal class AiClassParser : IParserService<AiClass>
	{
		private const string StartPrefix = "class ";
		private const string EndPrefix = "class_end";
		// private readonly ModelMapper<AiClassDto> _mapper = new ModelMapper<AiClassDto>();

		public IEnumerable<AiClass> Do(IEnumerable<string> rawData)
		{
			// bool onClassReading = false;
			bool onClassParametersReading = false;
			bool onClassHandlerReading = false;
			bool onClassHandlerVariablesReading = false;

			AiHandler currentHandler = null;
			AiClass currentClass = null;

			foreach (string raw in rawData)
			{
				if (raw.StartsWith(StartPrefix))
				{
					var parsed = AiParserUtils.ParseClassHeader(raw);
					if (parsed.IsNothing)
					{
						continue;
					}

					currentClass = parsed.GetValue();
					//onClassReading = true;
				}
				if (raw.StartsWith(EndPrefix))
				{
					yield return currentClass;

					currentClass = null;
					//onClassReading = false;
				}

				if (raw.StartsWith("parameter_define_begin"))
				{
					onClassParametersReading = true;
					continue;
				}

				if (raw.StartsWith("parameter_define_end"))
				{
					onClassParametersReading = false;
					continue;
				}

				if (raw.StartsWith("handler "))
				{
					Maybe<AiHandler> handler = AiParserUtils.ParseHandlerHeader(raw);
					if (handler.IsNothing)
					{
						continue;
					}

					currentHandler = handler.GetValue();
					currentClass.Handlers.Add(currentHandler);

					onClassHandlerReading = true;
					continue;
				}
				if (raw.StartsWith("handler_end"))
				{
					onClassHandlerReading = false;
					continue;
				}

				if (raw.StartsWith("variable_begin"))
				{
					onClassHandlerVariablesReading = true;
					continue;
				}

				if (raw.StartsWith("variable_end"))
				{
					onClassHandlerVariablesReading = false;
					continue;
				}

				if (onClassHandlerVariablesReading)
				{
					currentHandler.Variables.Add(raw);
				}
				else if (onClassHandlerReading)
				{
					currentHandler.Code.Add(raw);
				}
				else if(onClassParametersReading)
				{
					currentClass.Parameters.Add(AiParserUtils.ParseClassParameter(raw).GetValue());
				}
			}
			
			//IEnumerable<string> filteredData = Collect(rawData);
			//List<AiClass> dtoData = Parse(filteredData).ToList();

			//return result;
		}

		//private static IEnumerable<string> Collect(IEnumerable<string> lines)
		//{
		//	return ParseService.Collect(lines, StartPrefix, EndPrefix);
		//}

		//private IEnumerable<AiClass> Parse(IEnumerable<string> data)
		//{
		//	IEnumerable<AiClass> result = data.Select(Parse);
		//	return result;
		//}

		//private AiClass Parse(string record)
		//{
		//	ParsedData data = ParseService.ToKeyValueCollection(record);
		//	return _mapper.Map(data);
		//}
	}
}