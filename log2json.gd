extends SceneTree

func _init() -> void:
	var arguments = {}
	for argument in OS.get_cmdline_args():
		if argument.find("=") > -1:
			var key_value = argument.split("=")
			arguments[key_value[0].lstrip("--")] = key_value[1]
	
	if not main(arguments):
		quit(1)
	else:
		quit()

func main(arguments: Dictionary) -> bool:
	if not arguments.has('input'):
		print ("Must pass input file as --input=FILENAME")
		return false
	
	if not arguments.has('output'):
		print ("Must pass output file as --output=FILENAME")
		return false

	return log2json(arguments['input'], arguments['output'])

func log2json(input_filename: String, output_filename: String) -> bool:
	var infile := File.new()
	if not infile.file_exists(input_filename):
		print ("No such input file: %s" % input_filename)
		return false
	if infile.open_compressed(input_filename, File.READ, File.COMPRESSION_FASTLZ) != OK:
		print ("Unable to open input file: %s" % input_filename)
		return false
	
	var outfile := File.new()
	if outfile.open(output_filename, File.WRITE) != OK:
		infile.close()
		print ("Unable to open output file: %s" % output_filename)
		return false
	
	while not infile.eof_reached():
		var data = infile.get_var()
		outfile.store_line(JSON.print(data))
	
	infile.close()
	outfile.close()
	
	return true
