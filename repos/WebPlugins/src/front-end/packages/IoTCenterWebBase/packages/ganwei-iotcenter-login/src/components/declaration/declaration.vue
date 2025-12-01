<template>
    <!-- 服务条款 -->
    <div class="mask-layer">
        <aside class="tos" v-if="!isNext">
            <h3>{{ login?.title }}</h3>
            <div class="tos-main" v-infinite-scroll="load" infinite-scroll-distance="20">
                <p class="bd">{{ login?.topPart }}</p>
                <template v-for="(pact, index) in login?.pacts">
                    <h4 class="title">
                        {{ pact.title }}
                    </h4>
                    <declarationPacts :content="pact.content"></declarationPacts>
                </template>
            </div>
            <div class="bottom" v-if="bottomBtnShow" @click="next">
                {{ login?.bottomTips.next }}
            </div>
            <div v-else class="bottom">
                {{ login?.bottomTips.pleaseRead }}
                <i class="el-icon-bottom"></i>
                <i class="el-icon-bottom"></i>
                <i class="el-icon-bottom"></i>
            </div>
        </aside>
        <aside class="tos" v-else>
            <h3>{{ login?.privacyStatement.title }}</h3>
            <div class="tos-main" v-infinite-scroll="privacyLoad" infinite-scroll-distance="20">
                <p class="bd">{{ login?.privacyStatement.topPart }}</p>
                <template v-for="(pact, index) in login?.privacyStatement.pacts">
                    <h4 class="title">
                        {{ pact.title }}
                    </h4>
                    <declarationPacts :content="pact.content"></declarationPacts>
                </template>
            </div>

            <div class="bottom">
                <div class="pre" @click="pre">
                    {{ login?.bottomTips.previous }}
                </div>
                <el-checkbox @change="agreeChange" v-model="checkedAgree">{{ login?.bottomTips.agree
                }}</el-checkbox>
                <span>《{{ login?.title }}》《{{ login?.privacyStatement.title }}》</span>
                <span v-if="checkedAgree">（{{ agreeTime }}）</span>
            </div>
        </aside>
    </div>
</template>
<script>
import declarationPacts from './declarationPacts';

export default {
    components: {
        declarationPacts
    },

    data () {
        return {
            bottomBtnShow: false,
            privacyBottomBtnShow: false,
            checkedAgree: false,
            agreeTime: 5,
            tosShow: false,
            isNext: false,
            login: {}
        };
    },
    created () {
        const languagePackage = JSON.parse(sessionStorage.languagePackage) || {}
        const languageType = localStorage.languageType
        this.login = languagePackage?.[languageType]?.login?.serviceTerm || {}
        this.tosShow = true
    },

    methods: {
        next () {
            this.isNext = true
        },
        pre () {
            this.isNext = false
        },
        load () {
            this.bottomBtnShow = true
        },
        privacyLoad () {
            this.privacyBottomBtnShow = true
        },
        agreeChange () {
            this.agreeTime = 5
            if (this.checkedAgree) {
                this.agreeTimeQc = setInterval(() => {
                    this.agreeTime--;
                    if (this.agreeTime == 0) {
                        this.tosShow = true;
                        clearInterval(this.agreeTimeQc);
                        this.$emit('agree');
                    }
                }, 1000);
            } else {
                clearInterval(this.agreeTimeQc);
                this.agreeTime = 5;
                this.tosShow = false;
                this.checkedAgree = false;
            }
        }
    }
};
</script>
<style lang="scss" src="./declaration.scss" scoped />
