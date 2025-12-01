<template>
    <!-- 服务条款 -->
    <div class="mask-layer" v-if="!tosShow">
        <aside class="tos" v-if="!isNext">
            <h3>{{ serviceTerms.title }}</h3>
            <div class="tos-main" v-infinite-scroll="load" infinite-scroll-distance="20">
                <p class="bd">{{ serviceTerms.topPart }}</p>
                <template v-for="(pact, index) in serviceTerms.pacts">
                    <h4 class="title" :key="index">
                        {{ pact.title }}
                    </h4>
                    <declarationPacts :content="pact.content" :key="index"></declarationPacts>
                </template>
            </div>
            <!-- <div v-if="bottomBtnShow" class="bottom">
                <el-checkbox @change="agreeChange" v-model="checkedAgree">{{serviceTerms.bottomTips.agree}}</el-checkbox>
                <span>《{{serviceTerms.title}}》</span>
                <span v-if="checkedAgree">（{{ agreeTime }}）</span>
            </div> -->
            <div class="bottom" v-if="bottomBtnShow" @click="next">
                {{ serviceTerms.bottomTips.next }}
            </div>
            <div v-else class="bottom">{{ serviceTerms.bottomTips.pleaseRead }}<i class="el-icon-bottom"></i><i
                    class="el-icon-bottom"></i><i class="el-icon-bottom"></i></div>
        </aside>
        <aside class="tos" v-else>
            <h3>{{ serviceTerms.privacyStatement.title }}</h3>
            <div class="tos-main" v-infinite-scroll="privacyLoad" infinite-scroll-distance="20">
                <p class="bd">{{ serviceTerms.privacyStatement.topPart }}</p>
                <template v-for="(pact, index) in serviceTerms.privacyStatement.pacts">
                    <h4 class="title" :key="index">
                        {{ pact.title }}
                    </h4>
                    <declarationPacts :content="pact.content" :key="index"></declarationPacts>
                </template>
            </div>

            <div class="bottom">
                <div class="pre" @click="pre">
                    {{ serviceTerms.bottomTips.previous }}
                </div>
                <!-- v-if="privacyBottomBtnShow" -->
                <template>
                    <el-checkbox @change="agreeChange" v-model="checkedAgree">{{ serviceTerms.bottomTips.agree
                    }}</el-checkbox>
                    <span>《{{ serviceTerms.title }}》《{{ serviceTerms.privacyStatement.title }}》</span>
                    <span v-if="checkedAgree">（{{ agreeTime }}）</span>
                </template>
                <!-- <template v-else>
                    <div class="bottom">{{serviceTerms.bottomTips.pleaseRead}}<i class="el-icon-bottom"></i><i class="el-icon-bottom"></i><i class="el-icon-bottom"></i></div>
                </template> -->
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
    props: {
        serviceTerms: {
            type: Object,
            default: () => { return {} }
        }
    },
    data() {
        return {
            bottomBtnShow: false,
            privacyBottomBtnShow: false,
            checkedAgree: false,
            agreeTime: 5,
            tosShow: false,
            isNext: false
        };
    },
    mounted() {
        console.log(this.serviceTerms);
    },

    methods: {
        next() {
            this.isNext = true
        },
        pre() {
            this.isNext = false
        },
        load() {
            this.bottomBtnShow = true
        },
        privacyLoad() {
            this.privacyBottomBtnShow = true
        },
        agreeChange() {
            this.agreeTime = 5;
            if (this.checkedAgree) {
                this.agreeTimeQc = setInterval(() => {
                    this.agreeTime--;
                    if (this.agreeTime == -1) {
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
<style lang="scss" src="./declaration.scss" scoped>