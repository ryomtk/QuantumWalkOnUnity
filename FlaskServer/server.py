#!/usr/bin/env python3
import re
from flask import request
from flask import jsonify
from flask import Flask
from numpy import true_divide
from numpy.lib.histograms import _search_sorted_inclusive
from api import run_qasm, run_walk,reset_cat
import json

app = Flask(__name__)

@app.route('/')
def welcome():
    return "Hi Qiskiter!"

@app.route('/api/run/qasm', methods=['POST'])
def qasm():
    qasm = request.form.get('qasm')
    print("--------------")
    print (qasm)
    print(request.get_data())
    print (request.form)
    backend = 'qasm_simulator'
    output = run_qasm(qasm, backend)
    ret = {"result": output}
    return jsonify(ret)

@app.route('/api/run/walk', methods=['POST'])
def walker():
    times = request.form.get('walk_times')
    repeat = request.form.get('if_repeat')
    digit_num = request.form.get('digit_num')
    shots = request.form.get('cat_num')
    print("\n\n\n ___WALK START___")

    
    print("walk" + times + "times")
    print("repeat:" + repeat)
    print("digit of" + digit_num)
    print("with " + shots + " cats")

    if(repeat == "false"):
        repeat = False
    else:
        repeat = True

    if(digit_num != None):
        digit_num = int(digit_num)
    else:
        #万が一入力されてなければ4に設定
        digit_num = 4

    if(shots != None):
        shots = int(shots)
    else:
        shots = 1000

    
    output = run_walk(int(times),repeat,digit_num,shots)
    
    return jsonify(output)

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=8001)


@app.route('/api/run/reset', methods=['POST'])
def resetWalkCircuit():
    resetNum = request.form.get("digit_num")
    resetQWC(resetNum)
