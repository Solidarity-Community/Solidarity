import { apiValueConstructor, ApiValueConstructor } from '../API'

export const model = (csharpTypeName: string) => {
	return (Constructor: Constructor<unknown>) => {
		ModelConstructor.modelConstructorsByCsharpTypeName.set(csharpTypeName, Constructor)
	}
}

@apiValueConstructor()
export class ModelConstructor implements ApiValueConstructor<object, object> {
	static readonly modelConstructorsByCsharpTypeName = new Map<string, Constructor<unknown>>()
	private static readonly csharpTypeNameKeyName = '__typeName__'

	shallConstruct = (value: unknown) => !!value && typeof value === 'object' && ModelConstructor.csharpTypeNameKeyName in value

	construct(object: object) {
		const csharpTypeNameKey = ModelConstructor.csharpTypeNameKeyName as keyof typeof object
		const csharpTypeName = object[csharpTypeNameKey] as string
		const Constructor = ModelConstructor.modelConstructorsByCsharpTypeName.get(csharpTypeName)
		return !Constructor ? object : safeAssign(new Constructor, object)
	}
}

function safeAssign<T, U>(target: T, source: U) {
	const safeSource = Object.fromEntries(
		Object.entries(source).reduce((accumulator, currentValue) => {
			const descriptor = Object.getOwnPropertyDescriptor(Object.getPrototypeOf(target), currentValue[0])
			if (!descriptor || descriptor.set) {
				accumulator.push(currentValue)
			}
			return accumulator
		}, new Array<[string, any]>())
	)
	return Object.assign(target, safeSource)
}